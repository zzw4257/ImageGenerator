using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImageGenerator.Helpers;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
/// <summary>
/// Manages the user's favorite images.
/// </summary>
public class FavoriteController(IFavoriteService favoriteService) : ControllerBase
{
    private readonly IFavoriteService _favoriteService = favoriteService;

    /// <summary>
    /// Adds an image to the user's favorites.
    /// </summary>
    /// <param name="imageId">The ID of the image to add to favorites.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
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

    /// <summary>
    /// Removes an image from the user's favorites.
    /// </summary>
    /// <param name="imageId">The ID of the image to remove from favorites.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
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

    /// <summary>
    /// Gets a paginated list of the user's favorite images.
    /// </summary>
    /// <param name="param">The pagination parameters.</param>
    /// <returns>An <see cref="ActionResult"/> containing a list of <see cref="ImageDto"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<List<ImageDto>>> GetFavorites([FromQuery] PaginationBaseDto param)
    {
        try
        {
            var favorites = await _favoriteService.GetFavoriteImagesAsync(param);
            Response.Headers.AddPaginationHeader(favorites);
            return Ok(favorites.Items);
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
