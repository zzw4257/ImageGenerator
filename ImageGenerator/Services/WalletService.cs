using System.Security.Claims;
using AutoMapper;
using ImageGenerator.Database;
using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

public class WalletService(IgDbContext context, IHttpContextAccessor httpContextAccessor,IMapper mapper) : IWalletService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _http = httpContextAccessor;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Retrieves the user's current balance.
    /// </summary>
    /// <returns>A DTO representing the user's balance.</returns>
    public async Task<BalanceDto> GetBalanceAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == userId) 
            ?? throw new InvalidOperationException("用户不存在");

        return new BalanceDto
        {
            Balance = user.Credits
        };
    }

    /// <summary>
    /// Retrieves the user's transaction history, optionally filtered by type.
    /// </summary>
    /// <param name="type">Optional transaction type filter.</param>
    /// <returns>An array of DTOs representing the user's transactions.</returns>
    public async Task<TransactionDto[]> GetTransactionsAsync(TransactionType? type = null)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");

        var query = _context.Transactions
            .Where(t => t.CreatorId == userId);

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToArrayAsync();

        return _mapper.Map<TransactionDto[]>(transactions);
    }

    public async Task<TransactionDto> GetTransactionAsync(Guid transactionId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");

        var transaction = await _context.Transactions
            .Where(t => t.Id == transactionId && t.CreatorId == userId)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("交易不存在");

        return _mapper.Map<TransactionDto>(transaction);
    }

    /// <summary>
    /// Grants credits to the user's wallet.
    /// </summary>
    /// <param name="amount">The amount of credits to grant.</param>
    /// <returns>A DTO representing the created transaction.</returns>
    public async Task<TransactionDto> GrantAsync(decimal amount)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");

        if (amount <= 0)
        {
            throw new ArgumentException("金额必须大于 0", nameof(amount));
        }

        var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == userId) 
            ?? throw new InvalidOperationException("用户不存在");

        // Update user's credits
        user.Credits += amount;

        // Create transaction record
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Type = TransactionType.Recharge,
            Amount = amount,
            BalanceAfter = user.Credits,
            Description = $"充值 {amount} credits",
            CreatorId = userId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionDto
        {
            Type = transaction.Type,
            Amount = transaction.Amount,
            BalanceAfter = transaction.BalanceAfter,
            Description = transaction.Description,
            CreatorId = transaction.CreatorId
        };
    }

    /// <summary>
    /// 转账给指定用户
    /// </summary>
    public async Task<PayResponseDto> PayAsync(PayRequestDto request)
    {
        var senderId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");

        if (request.Amount <= 0)
        {
            throw new ArgumentException("转账金额必须大于 0", nameof(request.Amount));
        }

        if (senderId == request.RecipientUserId)
        {
            throw new InvalidOperationException("不能向自己转账");
        }

        // 使用事务确保转账的原子性
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 获取付款方
            var sender = await _context.Users!.FirstOrDefaultAsync(u => u.Id == senderId)
                ?? throw new InvalidOperationException("付款用户不存在");

            // 获取收款方
            var recipient = await _context.Users!.FirstOrDefaultAsync(u => u.Id == request.RecipientUserId)
                ?? throw new InvalidOperationException("收款用户不存在");

            // 检查余额
            if (sender.Credits < request.Amount)
            {
                throw new InvalidOperationException($"余额不足，当前余额: {sender.Credits} credits");
            }

            // 扣除付款方余额
            sender.Credits -= request.Amount;

            // 增加收款方余额
            recipient.Credits += request.Amount;

            // 创建付款交易记录
            var paymentTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Transfer,
                Amount = -request.Amount, // 负数表示支出
                BalanceAfter = sender.Credits,
                Description = $"转账给 {recipient.Username}{(string.IsNullOrEmpty(request.Note) ? "" : $": {request.Note}")}",
                CreatorId = senderId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // 创建收款交易记录
            var receiptTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Transfer,
                Amount = request.Amount, // 正数表示收入
                BalanceAfter = recipient.Credits,
                Description = $"收到来自 {sender.Username} 的转账{(string.IsNullOrEmpty(request.Note) ? "" : $": {request.Note}")}",
                CreatorId = request.RecipientUserId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Transactions.Add(paymentTransaction);
            _context.Transactions.Add(receiptTransaction);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new PayResponseDto
            {
                PaymentTransaction = _mapper.Map<TransactionDto>(paymentTransaction),
                SenderBalance = sender.Credits,
            };
        }
        catch (Exception)
        {
            // 回滚事务（如果异常发生在 Commit 之后，Rollback 会抛出异常但可以安全忽略）
            try
            {
                await transaction.RollbackAsync();
            }
            catch (InvalidOperationException)
            {
                // 事务已经完成，忽略 rollback 错误
            }
            throw;
        }
    }

    /// <summary>
    /// Gets the current user's ID from the HTTP context.
    /// </summary>
    /// <returns>The user's ID, or null if not authenticated.</returns>
    private Guid? GetCurrentUserId()
    {
        var val = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return Guid.TryParse(val, out var id) ? id : null;
    }
}