// D:\...\ImageGenerator\ImageGenerator\Controllers\RankingController.cs

using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

/// <summary>
/// 提供排行榜相关的 API 端点
/// </summary>
[ApiController]
[Route("api/ranking")]
public class RankingController(IRankingService rankingService) : ControllerBase
{
    private readonly IRankingService _rankingService = rankingService;

    /// <summary>
    /// 获取每日热门预制菜ID列表 (基于过去 24 小时收藏数)
    /// </summary>
    /// <param name="count">返回数量，默认为 10</param>
    /// <returns>200 OK + 热门 Preset 列表</returns>
    [HttpGet("trending/daily")] // 路由: GET /api/ranking/trending/daily
    // 这个接口是公开的，不需要 [Authorize]
    public async Task<ActionResult<IEnumerable<Guid>>> GetDailyTrending([FromQuery] int count = 10)
    {
        var trendingPresetIds = await _rankingService.GetDailyTrendingAsync(count);
        return Ok(trendingPresetIds); // Service 返回的就是 ID 列表，直接 OK
    }

    // (未来可以添加每周热门 GET /trending/weekly 等)
}