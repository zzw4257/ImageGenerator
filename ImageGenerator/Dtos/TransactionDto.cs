using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

public class TransactionDto: ActionBaseDto
{
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = string.Empty;
    public UserDto Creator { get; set; } = null!;
    public Guid CreatorId { get; set; }
}