using ImageGenerator.Interface;
using ImageGenerator.Dtos;
using ImageGenerator.Models;
using ImageGenerator.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using ImageGenerator.Helpers;


namespace ImageGenerator.Services;

/// <summary>
/// Provides services for managing conversations.
/// </summary>
public class ConversationService(IgDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IConversationService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;

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
