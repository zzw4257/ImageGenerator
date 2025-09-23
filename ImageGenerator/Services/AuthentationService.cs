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

public class AuthenticationService(IgDbContext context, JwtHelper jwtHelper) : IAuthenticationService
{
    private readonly IgDbContext _context = context;
    private readonly JwtHelper _jwtHelper = jwtHelper;

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
            Salt = salt
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