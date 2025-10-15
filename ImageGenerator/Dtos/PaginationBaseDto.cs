namespace ImageGenerator.Dtos;

/// <summary>
/// A base class for pagination data transfer objects.
/// </summary>
public class PaginationBaseDto
{
    /// <summary>
    /// The page number.
    /// </summary>
    public int PageNumber { get; set; } = 0;

    /// <summary>
    /// The size of the page.
    /// </summary>
    public int PageSize { get; set; } = 10;
}