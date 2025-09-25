namespace ImageGenerator.Dtos;

public class ProfileDto: ActionBaseDto
{
    public string Username { get; set; } = string.Empty;
    public int Credits { get; set; }
    public DateTime? LastCreditClaimedAt { get; set; }
}
