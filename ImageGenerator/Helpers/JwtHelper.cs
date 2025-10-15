using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ImageGenerator.Helpers;

/// <summary>
/// A helper class for generating JWT tokens.
/// </summary>
public class JwtHelper
{
    /// <summary>
    /// The JWT configuration.
    /// </summary>
    public JwtConfig? JwtConfig { get; set; }

    /// <summary>
    /// Generates a JWT token with the specified claims.
    /// </summary>
    /// <param name="claims">The claims to include in the token.</param>
    /// <returns>The generated JWT token.</returns>
    public string GetJwtToken(List<Claim> claims)
    {
        var jwtSecurityToken = new JwtSecurityToken(JwtConfig!.Issuer, JwtConfig.Audience, claims, JwtConfig.NotBefore, JwtConfig.Expiration, JwtConfig.SigningCredentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }
}
