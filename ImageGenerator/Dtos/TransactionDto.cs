using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

public class TransactionDto
{
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatorId { get; set; }
}