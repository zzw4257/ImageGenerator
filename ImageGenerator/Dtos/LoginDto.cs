using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

/// <summary>
/// Represents the data transfer object for a successful login.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// The ID of the logged-in user.
    /// </summary>
    public Guid UserId { get; set; } = Guid.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// The JWT token for the user's session.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The expiration time of the JWT token.
    /// </summary>
    public DateTime ExpirationTime { get; set; } = DateTime.MinValue;
}