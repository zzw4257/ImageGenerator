using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ImageGenerator.Interface;
using ImageGenerator.Dtos;
using ImageGenerator.Models;
using ImageGenerator.Helpers;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
/// <summary>
/// Manages conversations and image generation within them.
/// </summary>
public class ConversationController(IConversationService conversationService) : ControllerBase
{
    private readonly IConversationService _chatService = conversationService;

    /// <summary>
    /// Creates a new conversation for the current user.
    /// </summary>
    /// <returns>An <see cref="ActionResult"/> containing the created <see cref="ConversationDto"/>.</returns>
    [HttpPost("create")]
    public async Task<ActionResult<ConversationDto>> CreateConversation()
    {
        try
        {
            var result = await _chatService.CreateConversationAsync();
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest($"创建对话失败: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the details of a specific conversation.
    /// </summary>
    /// <param name="chatId">The ID of the conversation to retrieve.</param>
    /// <returns>An <see cref="ActionResult"/> containing the <see cref="ConversationDto"/>.</returns>
    [HttpGet("{chatId}")]
    public async Task<ActionResult<ConversationDto>> GetConversation(Guid chatId)
    {
        try
        {
            var result = await _chatService.GetConversationAsync(chatId);
            if (result == null)
            {
                return NotFound("对话不存在或无权访问");
            }
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest($"获取对话失败: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a paginated list of conversations for the current user.
    /// </summary>
    /// <param name="param">The pagination parameters.</param>
    /// <returns>An <see cref="ActionResult"/> containing a list of <see cref="ConversationDto"/>.</returns>
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetUserConversations([FromQuery] PaginationBaseDto param)
    {
        try
        {
            var result = await _chatService.GetUserConversationsAsync(param);
            Response.Headers.AddPaginationHeader(result);
            return Ok(result.Items);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest($"获取对话列表失败: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a specific conversation.
    /// </summary>
    /// <param name="chatId">The ID of the conversation to delete.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpDelete("{chatId}")]
    public async Task<IActionResult> DeleteConversation(Guid chatId)
    {
        try
        {
            await _chatService.DeleteConversationAsync(chatId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"删除对话失败: {ex.Message}");
        }
    }
}
