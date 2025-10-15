namespace ImageGenerator.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User: ModelBase
{
    /// <summary>
    /// The user's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's hashed password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The salt used for password hashing.
    /// </summary>
    public string? Salt { get; set; }

    /// <summary>
    /// The invitation that was used to register this user.
    /// </summary>
    public Invitation? InvitedBy { get; set; }

    /// <summary>
    /// The ID of the invitation that was used to register this user.
    /// </summary>
    public Guid? InvitedById { get; set; }

    /// <summary>
    /// The collection of conversations owned by this user.
    /// </summary>
    public ICollection<Conversation> Conversations { get; set; } = [];

    /// <summary>
    /// The number of credits the user has for image generation.
    /// </summary>
    public int Credits { get; set; } = 0;

    /// <summary>
    /// The timestamp when the user last claimed their credits.
    /// </summary>
    public DateTime? LastCreditClaimedAt { get; set; }
}