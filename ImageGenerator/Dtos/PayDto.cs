namespace ImageGenerator.Dtos;

/// <summary>
/// 转账请求 DTO
/// </summary>
public class PayRequestDto
{
    /// <summary>
    /// 接收者用户 ID
    /// </summary>
    public Guid RecipientUserId { get; set; }

    /// <summary>
    /// 转账金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 转账备注（可选）
    /// </summary>
    public string? Note { get; set; }
}

/// <summary>
/// 转账响应 DTO
/// </summary>
public class PayResponseDto
{
    /// <summary>
    /// 付款交易记录
    /// </summary>
    public TransactionDto PaymentTransaction { get; set; } = null!;

    /// <summary>
    /// 付款后余额
    /// </summary>
    public decimal SenderBalance { get; set; }
}
