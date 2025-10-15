using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// Defines the contract for a service that stores and retrieves images.
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Uploads an image.
    /// </summary>
    /// <param name="uploadDto">The DTO containing the image to upload.</param>
    /// <returns>A DTO representing the uploaded image.</returns>
    Task<ImageDto> UploadAsync(UploadImageDto uploadDto);

    /// <summary>
    /// Retrieves a paginated list of the user's images.
    /// </summary>
    /// <param name="param">The pagination parameters.</param>
    /// <returns>A paged list of image DTOs.</returns>
    Task<PagedList<Image, ImageDto>> ListUserImagesAsync(PaginationBaseDto param);
}
