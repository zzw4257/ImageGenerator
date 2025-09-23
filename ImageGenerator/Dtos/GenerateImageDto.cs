using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

public class GenerateImageDto
{
    public string Prompt { get; set; } = string.Empty;
    public GenerationType GenerationType { get; set; } = GenerationType.TextToImage;
    public string? Quality { get; set; } = "standard"; // "standard" 或 "hd"
    public string? Style { get; set; } = "vivid"; // "vivid" 或 "natural"
    public List<Guid>? InputImageIds { get; set; } // 用于 ImageToImage
    public string? ClientType { get; set; } = "gemini"; // "openai" 或 "gemini"
}
