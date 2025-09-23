namespace ImageGenerator.Dtos;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime SubscriptionExpiration { get; set; }
    public int Credits { get; set; }
    public DateTime? LastCreditClaimedAt { get; set; }
}
