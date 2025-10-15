namespace ImageGenerator.Dtos;

/// <summary>
/// A base class for data transfer objects, providing common properties.
/// </summary>
public class ActionBaseDto
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The timestamp of creation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}