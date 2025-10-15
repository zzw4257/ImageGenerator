namespace ImageGenerator.Dtos;

/// <summary>
/// Represents the data transfer object for a login form.
/// </summary>
public class LoginFormDto
{
    /// <summary>
    /// The user's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}