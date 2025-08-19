using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ImageGenerator.Helpers;

public class JwtHelper
{
    public JwtConfig? JwtConfig { get; set; }

    public string GetJwtToken(List<Claim> claims)
    {
        var jwtSecurityToken = new JwtSecurityToken(JwtConfig!.Issuer, JwtConfig.Audience, claims, JwtConfig.NotBefore, JwtConfig.Expiration, JwtConfig.SigningCredentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }
}
