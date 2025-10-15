using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

/// <summary>
/// Represents the data transfer object for generating an image.
/// </summary>
public class GenerateImageDto
{
    /// <summary>
    /// The prompt for the image generation.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The type of generation (e.g., TextToImage).
    /// </summary>
    public GenerationType GenerationType { get; set; } = GenerationType.TextToImage;

    /// <summary>
    /// The quality of the image ("standard" or "hd").
    /// </summary>
    public string? Quality { get; set; } = "standard";

    /// <summary>
    /// The style of the image ("vivid" or "natural").
    /// </summary>
    public string? Style { get; set; } = "vivid";

    /// <summary>
    /// The list of input image IDs for ImageToImage generation.
    /// </summary>
    public List<Guid>? InputImageIds { get; set; }

    /// <summary>
    /// The type of client to use for generation ("openai" or "gemini").
    /// </summary>
    public string? ClientType { get; set; } = "gemini";
}
