using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ImageGenerator.Helpers;

public class JwtConfig
{
    public string? SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int Expired { get; set; }
    public DateTime NotBefore => DateTime.Now;
    public DateTime Expiration => DateTime.Now.AddMinutes(Expired);
    private SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey ?? throw new NullReferenceException(nameof(SecretKey))));
    public SigningCredentials SigningCredentials => new(SigningKey, SecurityAlgorithms.HmacSha256);
    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.UTF8.GetBytes(SecretKey ?? throw new NullReferenceException(nameof(SecretKey))));
}
