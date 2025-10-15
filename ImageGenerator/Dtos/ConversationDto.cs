using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

/// <summary>
/// Represents a conversation data transfer object.
/// </summary>
public class ConversationDto: ActionBaseDto
{
    /// <summary>
    /// The title of the conversation.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp of the last update.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The list of generation records in the conversation.
    /// </summary>
    public List<GenerationRecordDto> GenerationRecords { get; set; } = [];
}

/// <summary>
/// Represents a generation record data transfer object.
/// </summary>
public class GenerationRecordDto: ActionBaseDto
{
    /// <summary>
    /// The type of image generation.
    /// </summary>
    public GenerationType GenerationType { get; set; }

    /// <summary>
    /// The prompt used for generation.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The generation parameters.
    /// </summary>
    public string GenerationParams { get; set; } = string.Empty;

    /// <summary>
    /// The status of the generation.
    /// </summary>
    public GenerationStatus Status { get; set; }

    /// <summary>
    /// The completion timestamp.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// The list of input images.
    /// </summary>
    public List<ImageDto> InputImages { get; set; } = [];

    /// <summary>
    /// The output image.
    /// </summary>
    public ImageDto? OutputImage { get; set; }
}

/// <summary>
/// Represents an image data transfer object.
/// </summary>
public class ImageDto: ActionBaseDto
{
    /// <summary>
    /// The path to the image.
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>
    /// A flag indicating if the image is a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// The size of the image in bytes.
    /// </summary>
    public long Size { get; set; }
}

/// <summary>
/// Represents an upload image data transfer object.
/// </summary>
public class UploadImageDto
{
    /// <summary>
    /// The image file to upload.
    /// </summary>
    public IFormFile File { get; set; } = default!;
}
