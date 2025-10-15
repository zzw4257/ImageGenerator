using ImageGenerator.Interface;
using ImageGenerator.Dtos;
using ImageGenerator.Models;
using ImageGenerator.Database;
using ImageGenerator.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using OpenAI.Images;
using ImageGenerator.Helpers;


namespace ImageGenerator.Services;

/// <summary>
/// Provides services for managing conversations and generating images.
/// </summary>
public class ConversationService(IgDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, ImageGenerationClientFactory clientFactory) : IConversationService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;
    private readonly ImageGenerationClientFactory _clientFactory = clientFactory;

    /// <summary>
    /// Creates a new conversation for the current user.
    /// </summary>
    /// <returns>A <see cref="ConversationDto"/> representing the newly created conversation.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated.</exception>
    public async Task<ConversationDto> CreateConversationAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");
        var conversation = new Conversation
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return _mapper.Map<ConversationDto>(conversation);
    }

    /// <summary>
    /// Retrieves a specific conversation for the current user.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation to retrieve.</param>
    /// <returns>A <see cref="ConversationDto"/> if found; otherwise, null.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated.</exception>
    public async Task<ConversationDto?> GetConversationAsync(Guid conversationId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");
        var conversation = await _context.Conversations
            .Include(c => c.GenerationRecords.OrderBy(gr => gr.CreatedAt))
                .ThenInclude(gr => gr.InputImages)
            .Include(c => c.GenerationRecords.OrderBy(gr => gr.CreatedAt))
                .ThenInclude(gr => gr.OutputImages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation == null)
            return null;

        return _mapper.Map<ConversationDto>(conversation);
    }

    /// <summary>
    /// Retrieves a paginated list of conversations for the current user.
    /// </summary>
    /// <param name="param">The pagination parameters.</param>
    /// <returns>A <see cref="PagedList{T, TDto}"/> of conversations.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated.</exception>
    public async Task<PagedList<Conversation, ConversationDto>> GetUserConversationsAsync(PaginationBaseDto param)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");
        var conversations = _context.Conversations
            .Include(c => c.GenerationRecords.OrderByDescending(gr => gr.CreatedAt))
            .ThenInclude(gr => gr.InputImages)
            .Include(c => c.GenerationRecords.OrderByDescending(gr => gr.CreatedAt))
            .ThenInclude(gr => gr.OutputImages)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAt);

        return await PagedList<Conversation, ConversationDto>.CreateAsync(conversations.AsQueryable(), param, _mapper);
    }

    /// <summary>
    /// Generates an image within a specified conversation, deducting credits from the user.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation in which to generate the image.</param>
    /// <param name="generateDto">The DTO containing image generation parameters.</param>
    /// <returns>A <see cref="GenerationRecordDto"/> representing the initial record of the image generation task.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated.</exception>
    /// <exception cref="ArgumentException">Thrown if the conversation does not exist or the user does not have access.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the user is not found or has insufficient credits.</exception>
    public async Task<GenerationRecordDto> GenerateImageAsync(Guid conversationId, GenerateImageDto generateDto)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");

        // Verify that the conversation exists and belongs to the current user
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId) ?? throw new ArgumentException("Conversation does not exist or access is denied.");

        // Calculate and validate credits (delegated to the specific client)
        var client = _clientFactory.GetClient(generateDto.ClientType ?? "gemini");
        var cost = client.GetCreditCost(generateDto);

        var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new InvalidOperationException("User not found.");
        if (user.Credits < cost)
        {
            throw new InvalidOperationException("Lack of Credits");
        }

        // Create generation record
        var generationRecord = await GenerateRecordFromDto(generateDto, conversationId);

        // Process input images (if ImageToImage)
        if (generateDto.GenerationType == GenerationType.ImageToImage && generateDto.InputImageIds?.Count > 0)
        {
            var inputImages = await _context.Images
                .Where(img => generateDto.InputImageIds.Contains(img.Id))
                .ToListAsync();

            generationRecord.InputImages = inputImages;
        }
        // 扣费 + 写入记录（事务）
        await DeductCreditsAndPersistAsync(user, cost, generationRecord, conversation);

        // 异步生成图片
        await ProcessImageGenerationAsync(generationRecord.Id, generateDto);

        return _mapper.Map<GenerationRecordDto>(generationRecord);
    }

    /// <summary>
    /// Deletes a conversation and its associated records and images.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation to delete.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated.</exception>
    /// <exception cref="ArgumentException">Thrown if the conversation does not exist or the user does not have access.</exception>
    public async Task DeleteConversationAsync(Guid conversationId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");

        var conversation = await _context.Conversations
            .Include(c => c.GenerationRecords)
            .ThenInclude(gr => gr.InputImages)
            .Include(c => c.GenerationRecords)
            .ThenInclude(gr => gr.OutputImages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId) ?? throw new ArgumentException("Conversation does not exist or access is denied.");

        // Delete associated image files
        foreach (var record in conversation.GenerationRecords)
        {
            foreach (var img in record.InputImages)
            {
                var inputImagePath = Path.Combine(Directory.GetCurrentDirectory(), img.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(inputImagePath))
                {
                    File.Delete(inputImagePath);
                }
                img.IsDeleted = true;
            }

            if (record.OutputImages != null)
            {
                var outputImagePath = Path.Combine(Directory.GetCurrentDirectory(), record.OutputImages.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }
                record.OutputImages.IsDeleted = true;
            }
            record.IsDeleted = true;
        }

        conversation.IsDeleted = true;
        // 删除对话及其生成记录
        await _context.SaveChangesAsync();
    }


    private async Task<GenerationRecord> GenerateRecordFromDto(GenerateImageDto generateDto, Guid conversationId)
    {
        var InputImages = new List<Image>();

        if (generateDto.InputImageIds != null)
        {
            foreach (var id in generateDto.InputImageIds)
            {
                var imgEntity = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
                if (imgEntity != null)
                {
                    var candidate = Path.Combine(Directory.GetCurrentDirectory(), imgEntity.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(candidate))
                    {
                        InputImages.Add(new Image { ImagePath = imgEntity.ImagePath });
                    }
                    continue;
                }
            }
        }

        return new GenerationRecord
        {
            ConversationId = conversationId, // 匿名用户没有对话
            GenerationType = generateDto.GenerationType,
            Prompt = generateDto.Prompt,
            GenerationParams = JsonSerializer.Serialize(new ImageGenerationParams
            {
                Quality = generateDto.Quality ?? "standard",
                Style = generateDto.Style ?? "vivid"
            }),
            Status = GenerationStatus.Pending,
            InputImages = InputImages,
            CreatedAt = DateTime.UtcNow
        };
    }

    // 上传图片方法（仅支持 jpg, png）
    // public async Task<ImageDto> UploadImageAsync(UploadImageDto uploadDto)
    // {
    //     var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未认证");
    //     var file = uploadDto.File;
    //     if (file == null || file.Length == 0)
    //         throw new ArgumentException("未选择文件");

    //     var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
    //     var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    //     if (!allowedExtensions.Contains(ext))
    //         throw new ArgumentException("仅支持 jpg, png 格式的图片");

    //     // 创建保存目录
    //     var currentDir = Directory.GetCurrentDirectory();
    //     var saveDirectory = Path.Combine(currentDir, "images", "uploads");
    //     Directory.CreateDirectory(saveDirectory);

    //     // 生成文件名
    //     var fileName = $"{Guid.NewGuid()}{ext}";
    //     var filePath = Path.Combine(saveDirectory, fileName);

    //     // 保存文件
    //     using (var stream = new FileStream(filePath, FileMode.Create))
    //     {
    //         await file.CopyToAsync(stream);
    //     }

    //     // 创建图片记录
    //     var image = new Image
    //     {
    //         IsFavorite = false,
    //         UserId = userId,
    //         ImagePath = Path.Combine("images", "uploads", fileName).Replace("\\", "/"),
    //     };

    //     _context.Images.Add(image);
    //     await _context.SaveChangesAsync();
    //     return _mapper.Map<ImageDto>(image);
    // }
    private async Task ProcessImageGenerationAsync(Guid generationRecordId, GenerateImageDto generateDto)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未认证");

        var generationRecord = await _context.GenerationRecords
            .Include(gr => gr.InputImages)
            .FirstOrDefaultAsync(gr => gr.Id == generationRecordId);

        if (generationRecord == null) return;

        try
        {
            // 更新状态为处理中
            generationRecord.Status = GenerationStatus.Processing;
            await _context.SaveChangesAsync();

            Console.WriteLine($"开始生成图片: {generationRecord.Prompt} (记录ID: {generationRecordId})");

            // 调用 Image Generation Client
            BinaryData imageBytes;
            var imageGenerationClient = _clientFactory.GetClient(generateDto.ClientType ?? "gemini");
            var options = imageGenerationClient.ConvertOptions(generateDto);

            if (generateDto.GenerationType == GenerationType.ImageToImage && generationRecord.InputImages.Count != 0)
            {
                imageBytes = await imageGenerationClient.GenerateImageFromImageAsync(generationRecord.Prompt, [.. generationRecord.InputImages], options);
            }
            else
            {
                imageBytes = await imageGenerationClient.GenerateImageAsync(generationRecord.Prompt, options);
            }

            var imagePath = SaveImage(imageBytes, generationRecordId);

            // 创建图片记录
            var image = new Image
            {
                ImagePath = imagePath,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Size = imageBytes.ToArray().LongLength,
                Type = ImageType.Generated
            };

            _context.Images.Add(image);
            generationRecord.OutputImages = image;
            generationRecord.Status = GenerationStatus.Completed;
            generationRecord.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            // 记录错误并更新状态
            generationRecord.Status = GenerationStatus.Failed;
            Console.WriteLine($"Image generation failed: {ex.Message}");
            // 失败返还 Credits
            await RefundCreditsOnFailureAsync(generationRecord, generateDto);
        }
        finally
        {
            await _context.SaveChangesAsync();
        }
    }

    private async Task DeductCreditsAndPersistAsync(User user, int cost, GenerationRecord generationRecord, Conversation conversation)
    {
        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            user.Credits -= cost;
            conversation.UpdatedAt = DateTime.UtcNow;
            _context.GenerationRecords.Add(generationRecord);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private async Task RefundCreditsOnFailureAsync(GenerationRecord generationRecord, GenerateImageDto generateDto)
    {
        try
        {
            var client = _clientFactory.GetClient(generateDto.ClientType ?? "gemini");
            var cost = client.GetCreditCost(generateDto);
            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == generationRecord.ConversationId);
            if (conversation != null)
            {
                var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == conversation.UserId);
                if (user != null)
                {
                    user.Credits += cost;
                }
            }
        }
        catch (Exception refundEx)
        {
            Console.WriteLine($"返还 credits 失败: {refundEx.Message}");
        }
    }

    private static string SaveImage(BinaryData bytes, Guid generationRecordId)
    {
        // 创建保存目录
        var currentDir = Directory.GetCurrentDirectory();
        var saveDirectory = Path.Combine(currentDir, "images", "generated");
        Directory.CreateDirectory(saveDirectory);

        // 生成文件名
        var fileName = $"{generationRecordId}_{DateTime.UtcNow:yyyyMMddHHmmss}.png";
        var filePath = Path.Combine(saveDirectory, fileName);

        // 下载图片
        using FileStream stream = File.OpenWrite(filePath);
        bytes.ToStream().CopyTo(stream);

        // 返回相对路径（用于数据库存储）
        return Path.Combine("images", "generated", fileName).Replace("\\", "/");
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}
