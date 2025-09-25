using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

public interface IFavoriteService
{
    Task AddToFavoritesAsync(Guid imageId);
    Task RemoveFromFavoritesAsync(Guid imageId);
    Task<PagedList<Image, ImageDto>> GetFavoriteImagesAsync(PaginationBaseDto param);
}
