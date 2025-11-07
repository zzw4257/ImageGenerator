using ImageGenerator.Enums;

namespace ImageGenerator.Models;

public class Transaction : ModelBase
{
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = string.Empty;
    public User Creator { get; set; } = null!;
    public Guid CreatorId { get; set; }
}