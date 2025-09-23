using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

public interface IConversationService
{
    Task<ConversationDto> CreateConversationAsync();
    Task<ConversationDto?> GetConversationAsync(Guid conversationId);
    Task<GenerationRecordDto> GenerateImageAsync(Guid conversationId, GenerateImageDto generateDto);
    Task<List<ConversationDto>> GetUserConversationsAsync();
    Task<ImageDto> UploadImageAsync(UploadImageDto uploadDto);
    Task DeleteConversationAsync(Guid conversationId);
}