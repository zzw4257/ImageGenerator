using ImageGenerator.Models;

namespace ImageGenerator.Interface;

public interface IInvitationService
{
    Task<List<Invitation>> GetInvitationCodeAsync();
    Task<Invitation> CreateInvitationAsync();
    Task DeleteInvitationAsync(Guid invitationId);
}