namespace ImageGenerator.Models;

public class User: ModelBase
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Salt { get; set; }
    public Invitation? InvitedBy { get; set; }
    public Guid? InvitedById { get; set; }
    public ICollection<Conversation> Conversations { get; set; } = [];
    public int Credits { get; set; } = 0;
    public DateTime? LastCreditClaimedAt { get; set; }
}