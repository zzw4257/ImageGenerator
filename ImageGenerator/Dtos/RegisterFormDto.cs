namespace ImageGenerator.Dtos;

/// <summary>
/// Represents the data transfer object for a registration form.
/// </summary>
public class RegisterFormDto
{
    /// <summary>
    /// The username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The invitation code.
    /// </summary>
    public string InvitationCode { get; set; } = string.Empty;
}