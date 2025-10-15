using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Interface;
using ImageGenerator.Database;
using ImageGenerator.Models;


namespace ImageGenerator.Services;

/// <summary>
/// Provides services for user authentication, including login and registration.
/// </summary>
public class AuthenticationService(IgDbContext context, JwtHelper jwtHelper) : IAuthenticationService
{
    private readonly IgDbContext _context = context;
    private readonly JwtHelper _jwtHelper = jwtHelper;

    /// <summary>
    /// Authenticates a user based on their username and password.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>A <see cref="LoginDto"/> containing the JWT token if authentication is successful; otherwise, null.</returns>
    /// <exception cref="ArgumentException">Thrown if the username or password is null or whitespace.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user is not found.</exception>
    public async Task<LoginDto> LoginAsync(string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var user = await _context.Users!
            .FirstOrDefaultAsync(x => x.Username == username) ?? throw new NullReferenceException(nameof(username));

        var passwordEncryption = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password + user.Salt)));

        if (user.Password != passwordEncryption)
            return null!;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Id.ToString()),
        };

        return new LoginDto
        {
            UserId = user.Id,
            Token = _jwtHelper.GetJwtToken(claims),
            ExpirationTime = DateTime.Now.AddMinutes(30)
        };
    }

    /// <summary>
    /// Registers a new user with the given username, password, and invitation code.
    /// </summary>
    /// <param name="username">The username for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="invitationCode">The invitation code required for registration.</param>
    /// <returns>A <see cref="LoginDto"/> containing the JWT token.</returns>
    /// <exception cref="ArgumentException">Thrown if the username, password, or invitation code is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the invitation code is invalid, expired, or the username already exists.</exception>
    public async Task<LoginDto> RegisterAsync(string username, string password, string invitationCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(invitationCode);

        var invitation = await _context.Invitations!
            .FirstOrDefaultAsync(x => x.Code == invitationCode && x.RemainingUses > 0) ?? throw new InvalidOperationException("Invalid or expired invitation code.");

        invitation.RemainingUses--;
        if (invitation.RemainingUses == 0)
            invitation.IsDeleted = true;

        var existingUser = await _context.Users!
            .FirstOrDefaultAsync(x => x.Username == username);
        if (existingUser != null)
            throw new InvalidOperationException("Username already exists.");

        var salt = Guid.NewGuid().ToString();
        var passwordEncryption = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password + salt)));

        var user = new User
        {
            Username = username,
            Password = passwordEncryption,
            Salt = salt,
            Credits = 100,
            LastCreditClaimedAt = DateTime.UtcNow
        };

        _context.Users!.Add(user);

        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Id.ToString()),
        };

        return new LoginDto
        {
            UserId = user.Id,
            Token = _jwtHelper.GetJwtToken(claims),
            ExpirationTime = DateTime.Now.AddMinutes(30)
        };
    }
}