using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Interface
{
    public interface IImageGenerationClient
    {
        Task<BinaryData> GenerateImageAsync(string prompt, object options);
        Task<BinaryData> GenerateImageFromImageAsync(string prompt, Image[] images, object options);
        object ConvertOptions(GenerateImageDto generateDto);
        int GetCreditCost(GenerateImageDto generateDto);
    }
}
