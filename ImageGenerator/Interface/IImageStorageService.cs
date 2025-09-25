using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

public interface IImageStorageService
{
    Task<ImageDto> UploadAsync(UploadImageDto uploadDto);
    Task<PagedList<Image, ImageDto>> ListUserImagesAsync(PaginationBaseDto param);
}
