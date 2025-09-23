using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;

    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        var profile = await _profileService.GetProfileAsync();
        return Ok(profile);
    }

    [HttpPost("credits/claim")]
    public async Task<ActionResult<ProfileDto>> ClaimDailyCredits()
    {
        try
        {
            var updated = await _profileService.ClaimDailyCreditsAsync();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
