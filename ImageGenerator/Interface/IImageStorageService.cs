using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

public interface IImageStorageService
{
    Task<ImageDto> UploadAsync(UploadImageDto uploadDto);
    Task<List<ImageDto>> ListUserImagesAsync();
}
