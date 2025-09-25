using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Models;

public class Conversation: ModelBase
{
    [Required]
    public Guid UserId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public ICollection<GenerationRecord> GenerationRecords { get; set; } = [];
}