using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ImageGenerator.Helpers;

/// <summary>
/// Represents the configuration for JWT.
/// </summary>
public class JwtConfig
{
    /// <summary>
    /// The secret key for signing the token.
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// The issuer of the token.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// The audience of the token.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// The expiration time in minutes.
    /// </summary>
    public int Expired { get; set; }

    /// <summary>
    /// The "not before" time for the token.
    /// </summary>
    public DateTime NotBefore => DateTime.Now;

    /// <summary>
    /// The expiration time for the token.
    /// </summary>
    public DateTime Expiration => DateTime.Now.AddMinutes(Expired);

    /// <summary>
    /// The signing key.
    /// </summary>
    private SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey ?? throw new NullReferenceException(nameof(SecretKey))));

    /// <summary>
    /// The signing credentials.
    /// </summary>
    public SigningCredentials SigningCredentials => new(SigningKey, SecurityAlgorithms.HmacSha256);

    /// <summary>
    /// The symmetric security key.
    /// </summary>
    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.UTF8.GetBytes(SecretKey ?? throw new NullReferenceException(nameof(SecretKey))));
}
