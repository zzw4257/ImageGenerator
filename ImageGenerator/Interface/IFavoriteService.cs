using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// Defines the contract for the favorite service.
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// Adds an image to the user's favorites.
    /// </summary>
    /// <param name="imageId">The ID of the image to add.</param>
    Task AddToFavoritesAsync(Guid imageId);

    /// <summary>
    /// Removes an image from the user's favorites.
    /// </summary>
    /// <param name="imageId">The ID of the image to remove.</param>
    Task RemoveFromFavoritesAsync(Guid imageId);

    /// <summary>
    /// Retrieves a paginated list of the user's favorite images.
    /// </summary>
    /// <param name="param">The pagination parameters.</param>
    /// <returns>A paged list of favorite image DTOs.</returns>
    Task<PagedList<Image, ImageDto>> GetFavoriteImagesAsync(PaginationBaseDto param);
}
