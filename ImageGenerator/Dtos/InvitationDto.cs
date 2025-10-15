namespace ImageGenerator.Dtos;

/// <summary>
/// Represents an invitation data transfer object.
/// </summary>
public class InvitationDto
{
    /// <summary>
    /// The unique identifier for the invitation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The invitation code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the invitation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// The number of remaining uses for the invitation.
    /// </summary>
    public int RemainingUses { get; set; } = 3;

    /// <summary>
    /// The ID of the user who issued the invitation.
    /// </summary>
    public Guid IssuerId { get; set; }
}