using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

public interface IFavoriteService
{
    Task AddToFavoritesAsync(Guid imageId);
    Task RemoveFromFavoritesAsync(Guid imageId);
    Task<List<ImageDto>> GetFavoriteImagesAsync();
}
