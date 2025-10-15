using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Interface
{
    /// <summary>
    /// Defines the contract for a client that generates images.
    /// </summary>
    public interface IImageGenerationClient
    {
        /// <summary>
        /// Generates an image from a text prompt.
        /// </summary>
        /// <param name="prompt">The text prompt.</param>
        /// <param name="options">The client-specific generation options.</param>
        /// <returns>A <see cref="BinaryData"/> object containing the image.</returns>
        Task<BinaryData> GenerateImageAsync(string prompt, object options);

        /// <summary>
        /// Generates an image from a text prompt and a set of input images.
        /// </summary>
        /// <param name="prompt">The text prompt.</param>
        /// <param name="images">The input images.</param>
        /// <param name="options">The client-specific generation options.</param>
        /// <returns>A <see cref="BinaryData"/> object containing the image.</returns>
        Task<BinaryData> GenerateImageFromImageAsync(string prompt, Image[] images, object options);

        /// <summary>
        /// Converts a generic DTO into a client-specific options object.
        /// </summary>
        /// <param name="generateDto">The DTO with the generation parameters.</param>
        /// <returns>A client-specific options object.</returns>
        object ConvertOptions(GenerateImageDto generateDto);

        /// <summary>
        /// Calculates the credit cost for a generation request.
        /// </summary>
        /// <param name="generateDto">The DTO with the generation parameters.</param>
        /// <returns>The cost in credits.</returns>
        int GetCreditCost(GenerateImageDto generateDto);
    }
}
