using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ImageGenerator.Controllers;

/// <summary>
/// 负责处理与预制菜(Presets)相关的 API 请求
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PresetsController(IPresetService presetService) : ControllerBase
{
    private readonly IPresetService _presetService = presetService;

    /// <summary>
    /// 获取所有可用的预制菜列表
    ///  GET /api/presets
    /// </summary>
    /// <returns>200 OK + 预制菜列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetPresets()
    {
        var presets = await _presetService.GetPresetsAsync();
        return Ok(presets);
    }

    /// <summary>
    /// 根据 ID 获取单个预制菜
    ///  GET /api/presets/{id}
    /// </summary>
    /// <param name="id">预制菜的 Guid ID</param>
    /// <returns>200 OK + Preset 对象, 或 404 Not Found</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPresetById(Guid id)
    {
        var preset = await _presetService.GetPresetByIdAsync(id);
        
        if (preset == null)
        {
            return NotFound(); // 返回 404
        }
        
        return Ok(preset); // 返回 200
    }

    /// <summary>
    /// 创建一个新的预制菜
    ///  POST /api/presets
    /// </summary>
    /// <param name="dto">来自请求体(Body)的预制菜数据</param>
    /// <returns>201 Created + 新创建的 Preset 对象</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePreset([FromBody] CreatePresetDto dto)
    {   
        try
        {
            var newPreset = await _presetService.CreatePresetAsync(dto);
            return CreatedAtAction(nameof(GetPresetById), new { id = newPreset.Id }, newPreset);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); 
        }
    }

    /// <summary>
    /// 软删除一个指定的预制菜
    ///  DELETE /api/presets/{id}
    /// </summary>
    /// <param name="id">要删除的预制菜 ID</param>
    /// <returns>204 No Content (成功) 或 404 Not Found (未找到)</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePreset(Guid id)
    {
        try
        {
            var success = await _presetService.DeletePresetAsync(id);
            if (!success)
            {
                return NotFound(); // 404
            }
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); 
        }
    }
}