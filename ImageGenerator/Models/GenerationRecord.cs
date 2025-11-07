using System.ComponentModel.DataAnnotations;
using ImageGenerator.Enums;

namespace ImageGenerator.Models;

/// <summary>
/// Represents the parameters for image generation.
/// </summary>
public class ImageGenerationParams
{
    /// <summary>
    /// The quality of the generated image.
    /// </summary>
    public string Quality { get; set; } = "standard";

    /// <summary>
    /// The style of the generated image.
    /// </summary>
    public string Style { get; set; } = "vivid";
}

/// <summary>
/// Represents a record of an image generation task.
/// </summary>
public class GenerationRecord: ModelBase
{
    /// <summary>
    /// The ID of the conversation this record belongs to.
    /// </summary>
    [Required]
    public Guid ConversationId { get; set; } = Guid.Empty;

    /// <summary>
    /// The ID of the Preset used for this generation, if any.
    /// </summary>
    public Guid? PresetId { get; set; } // '?' 表示它是可空的 (nullable)

    /// <summary>
    /// Navigation property to the Preset.
    /// </summary>
    public Preset? Preset { get; set; }

    /// <summary>
    /// The type of image generation.
    /// </summary>
    public GenerationType GenerationType { get; set; }

    /// <summary>
    /// The prompt used for image generation.
    /// </summary>
    [Required]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The JSON serialized generation parameters.
    /// </summary>
    [Required]
    public string GenerationParams { get; set; } = string.Empty;

    /// <summary>
    /// The status of the generation task.
    /// </summary>
    [Required]
    public GenerationStatus Status { get; set; }

    /// <summary>
    /// The timestamp when the generation was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// The conversation this record belongs to.
    /// </summary>
    public Conversation Conversation { get; set; } = null!;

    /// <summary>
    /// The collection of input images used for the generation.
    /// </summary>
    public ICollection<Image> InputImages { get; set; } = [];

    /// <summary>
    /// The output image of the generation.
    /// </summary>
    public Image? OutputImages { get; set; }
}