namespace ImageGenerator.Dtos;

/// <summary>
/// Represents a user profile data transfer object.
/// </summary>
public class ProfileDto: ActionBaseDto
{
    /// <summary>
    /// The username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's credit balance.
    /// </summary>
    public decimal Credits { get; set; }

    /// <summary>
    /// The timestamp of the last credit claim.
    /// </summary>
    public DateTime? LastCreditClaimedAt { get; set; }
}
