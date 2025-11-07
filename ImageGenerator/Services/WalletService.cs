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
            Id = transaction.Id,
            Type = transaction.Type,
            Amount = transaction.Amount,
            BalanceAfter = transaction.BalanceAfter,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            CreatorId = transaction.CreatorId
        };
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