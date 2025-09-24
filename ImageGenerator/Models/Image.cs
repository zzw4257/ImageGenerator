using ImageGenerator.Enums;

namespace ImageGenerator.Models;

public class Image
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.Empty;
    public ImageType Type { get; set; } = ImageType.Uploaded;
    public User User { get; set; } = null!;
    public required string ImagePath { get; set; } = string.Empty;
    public long Size { get; set; } = 0;
    public bool IsFavorite { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}