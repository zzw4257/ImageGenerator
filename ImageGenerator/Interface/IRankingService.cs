using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// 处理排行榜计算的服务接口
/// </summary>
public interface IRankingService
{
    /// <summary>
    /// 异步获取每日热门预制菜 ID 列表（基于收藏数）
    /// </summary>
    /// <param name="count">要获取的数量，默认为 10</param>
    /// <returns>热门 Preset ID 列表</returns>
    Task<IEnumerable<Guid>> GetDailyTrendingAsync(int count = 10);
}