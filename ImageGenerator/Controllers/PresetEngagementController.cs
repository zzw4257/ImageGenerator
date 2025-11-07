// D:\...\ImageGenerator\ImageGenerator\Controllers\PresetEngagementController.cs

using ImageGenerator.Enums;
using ImageGenerator.Helpers;
using ImageGenerator.Interface;
using ImageGenerator.Dtos;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

/// <summary>
/// 处理用户与预制菜互动（点赞、收藏、举报）的 API 端点
/// </summary>
[ApiController]
[Route("api/presets/{presetId}")] // <-- 基础路由设为 /api/presets/{presetId}
[RoleAuthorize(UserRole.User)] // User 及以上角色可访问
public class PresetEngagementController(IPresetEngagementService engagementService) : ControllerBase
{
    private readonly IPresetEngagementService _engagementService = engagementService;

    // --- 点赞 ---
    /// <summary>
    /// 点赞指定的预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>204 No Content (成功) 或 400 Bad Request (已点赞)</returns>
    [HttpPost("like")] // 路由: POST /api/presets/{presetId}/like
    public async Task<IActionResult> LikePreset(Guid presetId)
    {
        try
        {
            var success = await _engagementService.LikeAsync(presetId);
            if (!success)
            {
                return BadRequest(new { message = "Preset not found or already liked." });
            }
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 取消点赞指定的预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>204 No Content (成功) 或 404 Not Found (未点赞)</returns>
    [HttpDelete("like")] // 路由: DELETE /api/presets/{presetId}/like
    public async Task<IActionResult> UnlikePreset(Guid presetId)
    {
        try
        {
            var success = await _engagementService.UnlikeAsync(presetId);
            if (!success)
            {
                return NotFound(new { message = "Preset not found or not liked by user." }); // 404
            }
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 检查当前用户是否已点赞此预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>{"hasLiked": true/false}</returns>
    [HttpGet("like")] // 路由: GET /api/presets/{presetId}/like
    public async Task<IActionResult> GetLikeStatus(Guid presetId)
    {
        try
        {
            var hasLiked = await _engagementService.HasLikedAsync(presetId);
            return Ok(new { hasLiked }); // 返回简单的 JSON 对象
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // --- 收藏 ---
    /// <summary>
    /// 收藏指定的预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>204 No Content (成功) 或 400 Bad Request (已收藏)</returns>
    [HttpPost("favorite")] // 路由: POST /api/presets/{presetId}/favorite
    public async Task<IActionResult> FavoritePreset(Guid presetId)
    {
        try
        {
            var success = await _engagementService.FavoriteAsync(presetId);
            if (!success)
            {
                return BadRequest(new { message = "Preset not found or already favorited." });
            }
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 取消收藏指定的预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>204 No Content (成功) 或 404 Not Found (未收藏)</returns>
    [HttpDelete("favorite")] // 路由: DELETE /api/presets/{presetId}/favorite
    public async Task<IActionResult> UnfavoritePreset(Guid presetId)
    {
        try
        {
            var success = await _engagementService.UnfavoriteAsync(presetId);
            if (!success)
            {
                return NotFound(new { message = "Preset not found or not favorited by user." }); // 404
            }
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 检查当前用户是否已收藏此预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>{"hasFavorited": true/false}</returns>
    [HttpGet("favorite")] // 路由: GET /api/presets/{presetId}/favorite
    public async Task<IActionResult> GetFavoriteStatus(Guid presetId)
    {
        try
        {
            var hasFavorited = await _engagementService.HasFavoritedAsync(presetId);
            return Ok(new { hasFavorited }); // 返回简单的 JSON 对象
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // --- 举报 ---
    /// <summary>
    /// 举报指定的预制菜
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <param name="reportDto">举报信息 (原因和备注)</param>
    /// <returns>204 No Content (成功) 或 404 Not Found</returns>
    [HttpPost("report")] // 路由: POST /api/presets/{presetId}/report
    public async Task<IActionResult> ReportPreset(Guid presetId, [FromBody] ReportPresetDto reportDto)
    {
        try
        {
            var success = await _engagementService.ReportAsync(presetId, reportDto.Reason, reportDto.Notes);
            if (!success)
            {
                return NotFound(new { message = "Preset not found." }); // 404
            }
            return NoContent(); // 204 (举报成功通常不返回具体内容)
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}