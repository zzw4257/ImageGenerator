using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

/// <summary>
/// Request DTO for generating images.
/// </summary>
public class GenerateRequestDto
{
    /// <summary>
    /// The conversation ID to associate with.
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Optional preset ID to use.
    /// </summary>
    public Guid? PresetId { get; set; }

    /// <summary>
    /// The prompt for image generation.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The provider to use (e.g., "Stub", "Qwen", "Flux", "Gemini", "OpenAI").
    /// </summary>
    public string Provider { get; set; } = "Stub";

    /// <summary>
    /// Generation parameters as JSON string or object.
    /// </summary>
    public string? Params { get; set; }

    /// <summary>
    /// The list of input image IDs for ImageToImage generation.
    /// </summary>
    public List<Guid>? InputImageIds { get; set; }

    /// <summary>
    /// The quality of the image ("standard" or "hd").
    /// </summary>
    public string? Quality { get; set; } = "standard";

    /// <summary>
    /// The style of the image ("vivid" or "natural").
    /// </summary>
    public string? Style { get; set; } = "vivid";
}

/// <summary>
/// Response DTO for generation request.
/// </summary>
public class GenerateResponseDto
{
    /// <summary>
    /// The task ID for tracking generation status.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Estimated cost in credits.
    /// </summary>
    public decimal EstCost { get; set; }
}

/// <summary>
/// Response DTO for generation task status.
/// </summary>
public class GenerateTaskStatusDto
{
    /// <summary>
    /// The task ID.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// The current status of the task.
    /// </summary>
    public GenerationStatus Status { get; set; }

    /// <summary>
    /// The URL of the generated image (if succeeded).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Error message (if failed).
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// The prompt used for generation.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// The provider used.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// When the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the task was completed (if completed).
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}
