namespace ImageGenerator.Dtos;

public class ActionBaseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}