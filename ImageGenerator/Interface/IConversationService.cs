using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// Defines the contract for the conversation service.
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// Creates a new conversation.
    /// </summary>
    /// <returns>A DTO representing the new conversation.</returns>
    Task<ConversationDto> CreateConversationAsync();

    /// <summary>
    /// Retrieves a conversation by its ID.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation.</param>
    /// <returns>A DTO representing the conversation, or null if not found.</returns>
    Task<ConversationDto?> GetConversationAsync(Guid conversationId);

    /// <summary>
    /// Generates an image within a conversation.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation.</param>
    /// <param name="generateDto">The DTO with the generation parameters.</param>
    /// <returns>A DTO representing the generation record.</returns>
    Task<GenerationRecordDto> GenerateImageAsync(Guid conversationId, GenerateImageDto generateDto);

    /// <summary>
    /// Retrieves a paginated list of the user's conversations.
    /// </summary>
    /// <param name="param">The pagination parameters.</param>
    /// <returns>A paged list of conversation DTOs.</returns>
    Task<PagedList<Conversation, ConversationDto>> GetUserConversationsAsync(PaginationBaseDto param);

    /// <summary>
    /// Deletes a conversation.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation to delete.</param>
    Task DeleteConversationAsync(Guid conversationId);
}