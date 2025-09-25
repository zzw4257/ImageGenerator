using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

public interface IConversationService
{
    Task<ConversationDto> CreateConversationAsync();
    Task<ConversationDto?> GetConversationAsync(Guid conversationId);
    Task<GenerationRecordDto> GenerateImageAsync(Guid conversationId, GenerateImageDto generateDto);
    Task<PagedList<Conversation, ConversationDto>> GetUserConversationsAsync(PaginationBaseDto param);
    Task DeleteConversationAsync(Guid conversationId);
}