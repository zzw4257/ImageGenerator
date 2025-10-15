namespace ImageGenerator.Models;

/// <summary>
/// A base class for all models, providing common properties.
/// </summary>
public class ModelBase
{
    /// <summary>
    /// The unique identifier for the model.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The timestamp when the model was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// A flag indicating whether the model is deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}