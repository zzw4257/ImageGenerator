using System.ComponentModel.DataAnnotations;
using ImageGenerator.Enums;

namespace ImageGenerator.Models;

public class ImageGenerationParams
{
    public string Quality { get; set; } = "standard";
    public string Style { get; set; } = "vivid";
}

public class GenerationRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public Guid ConversationId { get; set; } = Guid.Empty;
    public GenerationType GenerationType { get; set; }
    [Required]
    public string Prompt { get; set; } = string.Empty;
    [Required]
    public string GenerationParams { get; set; } = string.Empty; // JSON serialized GenerationParams
    [Required]
    public GenerationStatus Status { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public ICollection<Image> InputImages { get; set; } = [];
    public Image? OutputImages { get; set; }
    public bool IsDeleted { get; set; } = false;
}