using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController(IFavoriteService favoriteService) : ControllerBase
{
    private readonly IFavoriteService _favoriteService = favoriteService;

    [HttpPost("{imageId}")]
    public async Task<ActionResult> AddToFavorites(Guid imageId)
    {
        try
        {
            await _favoriteService.AddToFavoritesAsync(imageId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpDelete("{imageId}")]
    public async Task<ActionResult> RemoveFromFavorites(Guid imageId)
    {
        try
        {
            await _favoriteService.RemoveFromFavoritesAsync(imageId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ImageDto>>> GetFavorites()
    {
        try
        {
            var favorites = await _favoriteService.GetFavoriteImagesAsync();
            return Ok(favorites);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
