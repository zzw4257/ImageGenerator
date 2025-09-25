namespace ImageGenerator.Dtos;

public class PaginationBaseDto
{
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}