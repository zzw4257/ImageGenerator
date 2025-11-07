using ImageGenerator.Enums;

namespace ImageGenerator.Interface;

/// <summary>
/// 处理用户与预制菜互动（点赞、收藏、举报）的服务接口
/// </summary>
public interface IPresetEngagementService
{
    /// <summary>
    /// 异步点赞一个预制菜。
    /// </summary>
    /// <param name="presetId">被点赞的预制菜 ID</param>
    /// <returns>操作是否成功（例如，如果已点赞则返回 false）</returns>
    Task<bool> LikeAsync(Guid presetId);

    /// <summary>
    /// 异步取消点赞一个预制菜。
    /// </summary>
    /// <param name="presetId">要取消点赞的预制菜 ID</param>
    /// <returns>操作是否成功（例如，如果未点赞则返回 false）</returns>
    Task<bool> UnlikeAsync(Guid presetId);

    /// <summary>
    /// 异步检查当前用户是否已点赞某个预制菜。
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>如果已点赞则返回 true，否则返回 false</returns>
    Task<bool> HasLikedAsync(Guid presetId);

    /// <summary>
    /// 异步收藏一个预制菜。
    /// </summary>
    /// <param name="presetId">被收藏的预制菜 ID</param>
    /// <returns>操作是否成功</returns>
    Task<bool> FavoriteAsync(Guid presetId);

    /// <summary>
    /// 异步取消收藏一个预制菜。
    /// </summary>
    /// <param name="presetId">要取消收藏的预制菜 ID</param>
    /// <returns>操作是否成功</returns>
    Task<bool> UnfavoriteAsync(Guid presetId);

    /// <summary>
    /// 异步检查当前用户是否已收藏某个预制菜。
    /// </summary>
    /// <param name="presetId">预制菜 ID</param>
    /// <returns>如果已收藏则返回 true，否则返回 false</returns>
    Task<bool> HasFavoritedAsync(Guid presetId);

    /// <summary>
    /// 异步举报一个预制菜。
    /// </summary>
    /// <param name="presetId">被举报的预制菜 ID</param>
    /// <param name="reason">举报原因</param>
    /// <param name="notes">（可选）补充说明</param>
    /// <returns>操作是否成功</returns>
    Task<bool> ReportAsync(Guid presetId, ReportReason reason, string? notes);
}