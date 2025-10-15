using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Models;

/// <summary>
/// Represents a conversation between a user and the AI, containing a series of generation records.
/// </summary>
public class Conversation: ModelBase
{
    /// <summary>
    /// The ID of the user who owns this conversation.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// The timestamp of the last update to the conversation.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The user who owns this conversation.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The collection of generation records within this conversation.
    /// </summary>
    public ICollection<GenerationRecord> GenerationRecords { get; set; } = [];
}