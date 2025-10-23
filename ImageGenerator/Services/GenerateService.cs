using System.Security.Claims;
using System.Text.Json;
using ImageGenerator.Database;
using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using ImageGenerator.Provider;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

/// <summary>
/// Service for handling image generation requests.
/// </summary>
public class GenerateService(
    IgDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    ImageProvider provider,
    IServiceScopeFactory scopeFactory) : IGenerateService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _http = httpContextAccessor;
    private readonly IConfiguration _configuration = configuration;
    private readonly ImageProvider _provider = provider;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    /// <summary>
    /// Submits a new image generation task.
    /// </summary>
    public async Task<GenerateResponseDto> GenerateAsync(GenerateRequestDto request)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");

        // 验证 Conversation 存在且属于当前用户
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && c.UserId == userId)
            ?? throw new ArgumentException("对话不存在或无权访问");

        var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new InvalidOperationException("用户不存在");

        // 估算费用
        var estCost = EstimateCost(request.Provider);

        // 余额校验
        if (user.Credits < estCost)
        {
            throw new InvalidOperationException($"余额不足。需要 {estCost} credits，当前余额 {user.Credits} credits");
        }

        // 确定生成类型
        var generationType = (request.InputImageIds != null && request.InputImageIds.Count > 0)
            ? GenerationType.ImageToImage
            : GenerationType.TextToImage;

        // 创建生成任务
        var taskId = Guid.NewGuid();
        var generationRecord = new GenerationRecord
        {
            Id = taskId,
            ConversationId = request.ConversationId,
            GenerationType = generationType,
            Prompt = request.Prompt,
            GenerationParams = JsonSerializer.Serialize(new ImageGenerationParams
            {
                Quality = request.Quality ?? "standard",
                Style = request.Style ?? "vivid"
            }),
            Status = GenerationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // 处理输入图片（如果是 ImageToImage）
        if (generationType == GenerationType.ImageToImage && request.InputImageIds != null)
        {
            var inputImages = await _context.Images
                .Where(img => request.InputImageIds.Contains(img.Id) && img.UserId == userId)
                .ToListAsync();

            if (inputImages.Count == 0)
            {
                throw new ArgumentException("未找到有效的输入图片");
            }

            generationRecord.InputImages = inputImages;
        }

        // 预扣款 + 创建交易记录（事务处理）
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            user.Credits -= estCost;
            conversation.UpdatedAt = DateTime.UtcNow;

            // 创建扣费交易记录
            var consumeTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Consume,
                Amount = -estCost,
                BalanceAfter = user.Credits,
                Description = $"生成图片 (Provider: {request.Provider}, TaskId: {taskId})",
                CreatorId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.GenerationRecords.Add(generationRecord);
            _context.Transactions.Add(consumeTransaction);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        // 异步处理图片生成 —— 使用独立的 DI scope 以避免使用已释放的 DbContext
        _ = Task.Run(() => ProcessGenerationAsync(taskId, request.Provider, userId))
            .ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    // TODO: Replace with proper logging if available
                    Console.Error.WriteLine($"Exception in ProcessGenerationAsync: {t.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

        return new GenerateResponseDto
        {
            TaskId = taskId,
            EstCost = estCost
        };
    }

    /// <summary>
    /// Gets the status of a generation task.
    /// </summary>
    public async Task<GenerateTaskStatusDto> GetTaskStatusAsync(Guid taskId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");

        var record = await _context.GenerationRecords
            .Include(r => r.OutputImages)
            .Include(r => r.Conversation)
            .FirstOrDefaultAsync(r => r.Id == taskId);

        if (record == null)
        {
            throw new InvalidOperationException("任务不存在");
        }

        // 验证用户是否有权访问此任务（通过 Conversation）
        if (record.Conversation == null || record.Conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("无权访问此任务");
        }

        return new GenerateTaskStatusDto
        {
            TaskId = record.Id,
            Status = record.Status,
            ImageUrl = record.OutputImages?.ImagePath,
            Error = record.Status == GenerationStatus.Failed ? "生成失败" : null,
            Prompt = record.Prompt,
            Provider = ExtractProvider(record.GenerationParams),
            CreatedAt = record.CreatedAt,
            CompletedAt = record.CompletedAt
        };
    }

    /// <summary>
    /// 估算生成费用
    /// </summary>
    private decimal EstimateCost(string provider)
    {
        var costs = _configuration.GetSection("CreditCosts").Get<Dictionary<string, decimal>>()
            ?? new Dictionary<string, decimal>();

        var key = $"{provider}.TextToImage";
        if (costs.TryGetValue(key, out var cost))
        {
            return cost;
        }

        // 默认费用
        return provider.ToLower() switch
        {
            "stub" => 0,
            "qwen" => 2,
            "flux" => 1,
            _ => 1
        };
    }

    /// <summary>
    /// 处理图片生成（真实的 Provider 调用）—— 在独立的 DI scope 中运行
    /// </summary>
    private async Task ProcessGenerationAsync(Guid taskId, string provider, Guid userId)
    {
        // 创建新的 DI scope 以获取独立的 DbContext 实例
        using var scope = _scopeFactory.CreateScope();
        var scopedContext = scope.ServiceProvider.GetRequiredService<IgDbContext>();

        try
        {
            var record = await scopedContext.GenerationRecords
                .Include(r => r.InputImages)
                .FirstOrDefaultAsync(r => r.Id == taskId);
            
            if (record == null) return;

            // 更新状态为处理中
            record.Status = GenerationStatus.Processing;
            await scopedContext.SaveChangesAsync();

            Console.WriteLine($"开始生成图片: {record.Prompt} (记录ID: {taskId}, Provider: {provider})");

            // 获取对应的 Provider 客户端
            var client = _provider.GetClient(provider);
            
            // 解析生成参数
            var generationParams = JsonSerializer.Deserialize<ImageGenerationParams>(record.GenerationParams) 
                ?? new ImageGenerationParams();

            // 准备生成参数
            var generateDto = new GenerateImageDto
            {
                Prompt = record.Prompt,
                GenerationType = record.GenerationType,
                ClientType = provider,
                Quality = generationParams.Quality,
                Style = generationParams.Style,
                InputImageIds = record.InputImages.Select(img => img.Id).ToList()
            };

            // 转换选项
            var options = client.ConvertOptions(generateDto);

            // 调用 Provider 生成图片
            BinaryData imageBytes;
            if (record.GenerationType == GenerationType.ImageToImage && record.InputImages.Count > 0)
            {
                imageBytes = await client.GenerateImageFromImageAsync(record.Prompt, [.. record.InputImages], options);
            }
            else
            {
                imageBytes = await client.GenerateImageAsync(record.Prompt, options);
            }

            // 保存图片
            var imagePath = SaveImage(imageBytes, taskId);

            // 创建图片记录
            var outputImage = new Image
            {
                Id = Guid.NewGuid(),
                ImagePath = imagePath,
                UserId = userId,
                IsFavorite = false,
                Type = ImageType.Generated,
                Size = imageBytes.ToArray().LongLength,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            scopedContext.Images.Add(outputImage);
            record.OutputImages = outputImage;
            record.Status = GenerationStatus.Completed;
            record.CompletedAt = DateTime.UtcNow;
            await scopedContext.SaveChangesAsync();

            Console.WriteLine($"图片生成成功: {imagePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"图片生成失败: {ex.Message}");
            
            var record = await scopedContext.GenerationRecords.FindAsync(taskId);
            if (record != null)
            {
                record.Status = GenerationStatus.Failed;
                record.CompletedAt = DateTime.UtcNow;
                await scopedContext.SaveChangesAsync();

                // 失败时退款
                await RefundCreditsAsync(taskId, ex.Message, scopedContext);
            }
        }
    }

    /// <summary>
    /// 保存生成的图片到本地
    /// </summary>
    private static string SaveImage(BinaryData bytes, Guid taskId)
    {
        // 创建保存目录
        var currentDir = Directory.GetCurrentDirectory();
        var saveDirectory = Path.Combine(currentDir, "images", "generated");
        Directory.CreateDirectory(saveDirectory);

        // 生成文件名
        var fileName = $"{taskId}_{DateTime.UtcNow:yyyyMMddHHmmss}.png";
        var filePath = Path.Combine(saveDirectory, fileName);

        // 保存图片
        using FileStream stream = File.OpenWrite(filePath);
        bytes.ToStream().CopyTo(stream);

        // 返回相对路径（用于数据库存储）
        return Path.Combine("images", "generated", fileName).Replace("\\", "/");
    }

    /// <summary>
    /// 失败时退款 —— 使用传入的 scoped DbContext
    /// </summary>
    private static async Task RefundCreditsAsync(Guid taskId, string reason, IgDbContext scopedContext)
    {
        try
        {
            // 找到原始扣款交易
            var consumeTransaction = await scopedContext.Transactions
                .Where(t => t.Type == TransactionType.Consume && t.Description.Contains(taskId.ToString()))
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (consumeTransaction == null) return;

            var refundUserId = consumeTransaction.CreatorId;
            var user = await scopedContext.Users!.FirstOrDefaultAsync(u => u.Id == refundUserId);
            if (user == null) return;

            var refundAmount = Math.Abs(consumeTransaction.Amount);
            user.Credits += refundAmount;

            var refundTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Refund,
                Amount = refundAmount,
                BalanceAfter = user.Credits,
                Description = $"生成失败退款 (TaskId: {taskId}, 原因: {reason})",
                CreatorId = refundUserId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            scopedContext.Transactions.Add(refundTransaction);
            await scopedContext.SaveChangesAsync();
        }
        catch
        {
            // 退款失败，记录日志但不影响主流程
        }
    }

    /// <summary>
    /// 从参数中提取 provider
    /// </summary>
    private static string ExtractProvider(string paramsJson)
    {
        try
        {
            var doc = JsonDocument.Parse(paramsJson);
            if (doc.RootElement.TryGetProperty("provider", out var provider))
            {
                return provider.GetString() ?? "Unknown";
            }
        }
        catch
        {
            // 解析失败
        }
        return "Unknown";
    }

    /// <summary>
    /// Gets the current user's ID from the HTTP context.
    /// </summary>
    private Guid? GetCurrentUserId()
    {
        var val = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return Guid.TryParse(val, out var id) ? id : null;
    }
}
