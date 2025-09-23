using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync();
    Task<ProfileDto> ClaimDailyCreditsAsync();
}
