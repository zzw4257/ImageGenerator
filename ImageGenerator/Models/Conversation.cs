using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Models;

public class Conversation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public ICollection<GenerationRecord> GenerationRecords { get; set; } = [];
    public bool IsDeleted { get; set; } = false;
}