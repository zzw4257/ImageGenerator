// D:\...\ImageGenerator\ImageGenerator\Services\RankingService.cs

using ImageGenerator.Database;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

/// <summary>
/// 处理排行榜计算的服务实现
/// </summary>
public class RankingService(IgDbContext context) : IRankingService
{
    private readonly IgDbContext _context = context;

    /// <summary>
    /// 异步获取每日热门预制菜列表（基于 24 小时内的收藏数）。
    /// </summary>
    public async Task<IEnumerable<Guid>> GetDailyTrendingAsync(int count = 10)
    {
        var timeCutoff = DateTime.UtcNow.AddHours(-24);

        // 查询过去 24 小时内各 Preset 被收藏的次数
        var trendingPresetIds = await _context.PresetFavorites
            .Where(f => !f.IsDeleted && f.CreatedAt > timeCutoff)
            .GroupBy(f => f.PresetId) // 按 PresetId 分组
            .Select(g => new { // 创建匿名对象
                PresetId = g.Key,
                RecentFavoriteCount = g.Count() // 计算每个组的数量
            })
            .OrderByDescending(x => x.RecentFavoriteCount) // 按数量降序排序
            .Take(count) // 取前 count 个
            .Select(x => x.PresetId) // 只选择 PresetId
            .ToListAsync();

        return trendingPresetIds;
    }
}