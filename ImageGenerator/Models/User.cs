namespace ImageGenerator.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime SubscriptionExpiration { get; set; } = DateTime.Now;
    public string? Salt { get; set; }
    public Invitation? InvitedBy { get; set; }
    public Guid? InvitedById { get; set; }
    public ICollection<Conversation> Conversations { get; set; } = [];
    public bool IsDeleted { get; set; } = false;
}