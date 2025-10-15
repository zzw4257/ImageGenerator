using ImageGenerator.Enums;

namespace ImageGenerator.Models;

/// <summary>
/// Represents an image in the system.
/// </summary>
public class Image: ModelBase
{
    /// <summary>
    /// The ID of the user who owns this image.
    /// </summary>
    public Guid UserId { get; set; } = Guid.Empty;

    /// <summary>
    /// The type of the image (e.g., uploaded or generated).
    /// </summary>
    public ImageType Type { get; set; } = ImageType.Uploaded;

    /// <summary>
    /// The user who owns this image.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The path to the image file.
    /// </summary>
    public required string ImagePath { get; set; } = string.Empty;

    /// <summary>
    /// The size of the image file in bytes.
    /// </summary>
    public long Size { get; set; } = 0;

    /// <summary>
    /// A flag indicating whether the image is marked as a favorite.
    /// </summary>
    public bool IsFavorite { get; set; } = false;
}