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


namespace ImageGenerator.Services;

public class ConversationService(IgDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration, HttpClient httpClient, ImageClient client) : IConversationService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;
    private readonly IConfiguration _configuration = configuration;
    private readonly HttpClient _httpClient = httpClient;
    private readonly ImageClient _client = client;

    public async Task<ConversationDto> CreateConversationAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未认证");
        var conversation = new Conversation
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return _mapper.Map<ConversationDto>(conversation);
    }

    public async Task<ConversationDto?> GetConversationAsync(Guid conversationId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未认证");
        var conversation = await _context.Conversations
            .Include(c => c.GenerationRecords)
                .ThenInclude(gr => gr.InputImages)
            .Include(c => c.GenerationRecords)
                .ThenInclude(gr => gr.OutputImages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation == null)
            return null;

        return _mapper.Map<ConversationDto>(conversation);
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未认证");
        var conversations = await _context.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<ConversationDto>>(conversations);
    }

    public async Task<GenerationRecordDto> GenerateImageAsync(Guid conversationId, GenerateImageDto generateDto)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未认证");

        // 验证对话是否存在且属于当前用户
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId) ?? throw new ArgumentException("对话不存在或无权访问");

        // 创建生成记录
        var generationRecord = GenerateRecordFromDto(generateDto, conversationId);

        // 处理输入图片（如果是 ImageToImage）
        if (generateDto.GenerationType == GenerationType.ImageToImage && generateDto.InputImageIds?.Count > 0)
        {
            var inputImages = await _context.GeneratedImages
                .Where(img => generateDto.InputImageIds.Contains(img.Id))
                .ToListAsync();

            generationRecord.InputImages = inputImages;
        }

        _context.GenerationRecords.Add(generationRecord);
        await _context.SaveChangesAsync();

        // 异步生成图片
        await ProcessImageGenerationAsync(generationRecord.Id, generateDto);

        return _mapper.Map<GenerationRecordDto>(generationRecord);
    }

    public async Task<GenerationRecordDto> GenerateImageAnonymousAsync(GenerateImageDto generateDto)
    {
        // 创建匿名生成记录（不关联用户或对话）
        var generationRecord = GenerateRecordFromDto(generateDto, Guid.Parse("00000000-0000-0000-0000-000000000004"));

        // 处理输入图片（如果是 ImageToImage）
        if (generateDto.GenerationType == GenerationType.ImageToImage && generateDto.InputImageIds?.Count > 0)
        {
            var inputImages = await _context.GeneratedImages
                .Where(img => generateDto.InputImageIds.Contains(img.Id))
                .ToListAsync();

            generationRecord.InputImages = inputImages;
        }

        _context.GenerationRecords.Add(generationRecord);
        await _context.SaveChangesAsync();

        // 异步生成图片
        await ProcessImageGenerationAsync(generationRecord.Id, generateDto);

        return _mapper.Map<GenerationRecordDto>(generationRecord);
    }

    private static GenerationRecord GenerateRecordFromDto(GenerateImageDto generateDto, Guid conversationId)
    {
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
            CreatedAt = DateTime.UtcNow
        };
    }

    private static ImageGenerationOptions GenerateImageOptionsFromDto(GenerateImageDto generateDto)
    {
        return new ImageGenerationOptions
        {
            Quality = generateDto.Quality?.ToLower() switch
            {
                "hd" => GeneratedImageQuality.High,
                "standard" => GeneratedImageQuality.Standard,
                _ => GeneratedImageQuality.Standard
            },
            Size = GeneratedImageSize.W1024xH1024,
            Style = generateDto.Style?.ToLower() switch
            {
                "natural" => GeneratedImageStyle.Natural,
                "vivid" => GeneratedImageStyle.Vivid,
                _ => GeneratedImageStyle.Vivid
            }
        };
    }

    private async Task ProcessImageGenerationAsync(Guid generationRecordId, GenerateImageDto generateDto)
    {
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

            // 调用 OpenAI DALL-E API
            var imageBytes = await CallApiAsync(generationRecord.Prompt, GenerateImageOptionsFromDto(generateDto));

            var imagePath = SaveImage(imageBytes, generationRecordId);

            // 创建图片记录
            var image = new Image
            {
                ImagePath = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.GeneratedImages.Add(image);
            generationRecord.OutputImages = image;
            generationRecord.Status = GenerationStatus.Completed;
            generationRecord.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            // 记录错误并更新状态
            generationRecord.Status = GenerationStatus.Failed;
            Console.WriteLine($"Image generation failed: {ex.Message}");
            // 可以在这里记录具体的错误信息
        }
        finally
        {
            await _context.SaveChangesAsync();
        }
    }

    private async Task<BinaryData> CallApiAsync(string Prompt, ImageGenerationOptions GenerationParams)
    {
        try
        {
            GeneratedImage image = await _client.GenerateImageAsync(Prompt);
            BinaryData bytes = image.ImageBytes;
            return bytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API 调用失败: {ex.Message}");
            throw;
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
