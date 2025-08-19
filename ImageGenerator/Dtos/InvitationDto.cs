namespace ImageGenerator.Dtos;

public class InvitationDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int RemainingUses { get; set; } = 3;
    public Guid IssuerId { get; set; }
}