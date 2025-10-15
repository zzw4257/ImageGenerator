using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// Defines the contract for the invitation service.
/// </summary>
public interface IInvitationService
{
    /// <summary>
    /// Retrieves a list of invitation codes.
    /// </summary>
    /// <returns>A list of invitations.</returns>
    Task<List<Invitation>> GetInvitationCodeAsync();

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <returns>The newly created invitation.</returns>
    Task<Invitation> CreateInvitationAsync();

    /// <summary>
    /// Deletes an invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to delete.</param>
    Task DeleteInvitationAsync(Guid invitationId);
}