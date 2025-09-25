namespace ImageGenerator.Models;

public class Invitation: ModelBase
{
    public string Code { get; set; } = string.Empty;
    public int RemainingUses { get; set; } = 3;
    public User Issuer { get; set; } = null!;
    public Guid IssuerId { get; set; } = Guid.Empty;
} 