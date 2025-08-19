namespace ImageGenerator.Dtos;

public class LoginDto
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationTime { get; set; } = DateTime.MinValue;
}