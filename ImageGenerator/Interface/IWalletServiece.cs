using ImageGenerator.Dtos;
using ImageGenerator.Enums;

namespace ImageGenerator.Interface;

public interface IWalletService
{
    /// <summary>
    /// Retrieves the user's wallet information.
    /// </summary>
    /// <returns>A DTO representing the user's wallet.</returns>
    Task<BalanceDto> GetBalanceAsync();

    /// <summary>
    /// Retrieves the user's transaction history, optionally filtered by type.
    /// </summary>
    /// <param name="type">Optional transaction type filter.</param>
    /// <returns>An array of transaction DTOs.</returns>
    Task<TransactionDto[]> GetTransactionsAsync(TransactionType? type = null);

    /// <summary>
    /// Retrieves a specific transaction by ID.
    /// </summary>
    /// <param name="transactionId">The transaction ID.</param>
    /// <returns>The transaction DTO.</returns>
    Task<TransactionDto> GetTransactionAsync(Guid transactionId);

    /// <summary>
    /// Grants credits to the user's wallet.
    /// </summary>
    /// <param name="amount">The amount of credits to grant.</param>
    /// <returns>The transaction DTO.</returns>
    Task<TransactionDto> GrantAsync(decimal amount);
}