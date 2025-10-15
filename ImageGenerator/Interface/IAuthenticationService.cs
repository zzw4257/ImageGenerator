using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

/// <summary>
/// Defines the contract for the authentication service.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>A DTO containing the login information.</returns>
    Task<LoginDto> LoginAsync(string username, string password);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="invitationCode">The invitation code.</param>
    /// <returns>A DTO containing the login information.</returns>
    Task<LoginDto> RegisterAsync(string username, string password, string invitationCode);
}
