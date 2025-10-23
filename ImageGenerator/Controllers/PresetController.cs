using ImageGenerator.Interface;
using Microsoft.AspNetCore.Mvc;

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
}