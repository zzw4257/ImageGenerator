using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api")]
[Authorize]
/// <summary>
/// Manages image generation requests.
/// </summary>
public class GenerateController(IGenerateService generateService) : ControllerBase
{
    private readonly IGenerateService _generateService = generateService;

    /// <summary>
    /// Submits a new image generation task within a conversation.
    /// POST /api/generate
    /// </summary>
    /// <param name="request">The generation request containing conversationId, presetId/prompt, provider, and params.</param>
    /// <returns>Task ID and estimated cost.</returns>
    [HttpPost("generate")]
    public async Task<ActionResult<GenerateResponseDto>> Generate([FromBody] GenerateRequestDto request)
    {
        try
        {
            var response = await _generateService.GenerateAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"生成失败: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gets the status of a generation task.
    /// GET /api/generate/{taskId}
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>Task status including status, imageUrl, and error if any.</returns>
    [HttpGet("generate/{taskId}")]
    public async Task<ActionResult<GenerateTaskStatusDto>> GetTaskStatus(Guid taskId)
    {
        try
        {
            var status = await _generateService.GetTaskStatusAsync(taskId);
            return Ok(status);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"查询失败: {ex.Message}" });
        }
    }
}
