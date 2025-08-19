namespace ImageGenerator.Models;

public class Invitation
{
    public Guid Id { get; set; }  = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int RemainingUses { get; set; } = 3;
    public User Issuer { get; set; } = null!;
    public Guid IssuerId { get; set; } = Guid.Empty;
    public bool IsDeleted { get; set; } = false;
} 