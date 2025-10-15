namespace ImageGenerator.Models;

/// <summary>
/// Represents an invitation code for user registration.
/// </summary>
public class Invitation: ModelBase
{
    /// <summary>
    /// The unique invitation code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The number of remaining uses for this invitation code.
    /// </summary>
    public int RemainingUses { get; set; } = 3;

    /// <summary>
    /// The user who issued this invitation.
    /// </summary>
    public User Issuer { get; set; } = null!;

    /// <summary>
    /// The ID of the user who issued this invitation.
    /// </summary>
    public Guid IssuerId { get; set; } = Guid.Empty;
}