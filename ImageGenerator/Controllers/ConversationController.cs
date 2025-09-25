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
public class ConversationController(IConversationService conversationService) : ControllerBase
{
    private readonly IConversationService _chatService = conversationService;

    /// <summary>
    /// 创建新的对话
    /// </summary>
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
    /// 获取指定对话的详细信息
    /// </summary>
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
    /// 在指定对话中生成图片
    /// </summary>
    [HttpPost("generate/{chatId}")]
    public async Task<ActionResult<GenerationRecordDto>> GenerateImage(Guid chatId, [FromBody] GenerateImageDto generateDto)
    {
        try
        {
            var result = await _chatService.GenerateImageAsync(chatId, generateDto);
            return Ok(result);
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
            return BadRequest($"生成图片失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取当前用户的所有对话列表
    /// </summary>
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
