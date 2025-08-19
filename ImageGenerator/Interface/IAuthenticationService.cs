using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

public interface IAuthenticationService
{
    Task<LoginDto> LoginAsync(string username, string password);
    Task<RegisterDto> RegisterAsync(string username, string password, string invitationCode);
}
