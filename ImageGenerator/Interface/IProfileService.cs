using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

/// <summary>
/// Defines the contract for the user profile service.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Retrieves the user's profile.
    /// </summary>
    /// <returns>A DTO representing the user's profile.</returns>
    Task<ProfileDto> GetProfileAsync();

    /// <summary>
    /// Allows the user to claim their daily credits.
    /// </summary>
    /// <returns>A DTO representing the updated user profile.</returns>
    Task<ProfileDto> ClaimDailyCreditsAsync();
}
