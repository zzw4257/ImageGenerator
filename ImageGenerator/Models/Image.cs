namespace ImageGenerator.Models;

public class Image
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string ImagePath { get; set; } = string.Empty;
    public bool IsFavorite { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}