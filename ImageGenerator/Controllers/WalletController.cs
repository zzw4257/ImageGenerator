using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImageGenerator.Helpers;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
/// <summary>
/// Manages the user's wallet.
/// </summary>
public class WalletController(IWalletService walletService) : ControllerBase
{
    private readonly IWalletService _walletService = walletService;

    /// <summary>
    /// Gets the user's current balance.
    /// </summary>
    /// <returns>The user's balance information.</returns>
    [HttpGet("balance")]
    public async Task<ActionResult<BalanceDto>> GetBalance()
    {
        try
        {
            var balance = await _walletService.GetBalanceAsync();
            return Ok(balance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gets the user's transaction history, optionally filtered by type.
    /// </summary>
    /// <param name="type">Optional transaction type filter.</param>
    /// <returns>A list of transactions.</returns>
    [HttpGet("transactions")]
    public async Task<ActionResult<TransactionDto[]>> GetTransactions([FromQuery] TransactionType? type = null)
    {
        try
        {
            var transactions = await _walletService.GetTransactionsAsync(type);
            return Ok(transactions);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("transactions/{transactionId}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(Guid transactionId)
    {
        try
        {
            var transaction = await _walletService.GetTransactionAsync(transactionId);
            return Ok(transaction);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Grants credits to the user's wallet.
    /// </summary>
    /// <param name="amount">The amount of credits to grant.</param>
    /// <returns>The transaction details.</returns>
    [HttpPost("grant")]
    public async Task<ActionResult<TransactionDto>> Grant([FromBody] decimal amount)
    {
        try
        {
            var transaction = await _walletService.GrantAsync(amount);
            return Ok(transaction);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
