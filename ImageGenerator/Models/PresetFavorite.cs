using ImageGenerator.Models;

namespace ImageGenerator.Models;

/// <summary>
/// 记录用户“收藏”预制菜的事件。
/// 用于计算 Trending 和“我的收藏”列表。
/// </summary>
public class PresetFavorite : ModelBase
{
    /// <summary>
    /// 收藏的用户 ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// 导航属性：收藏的用户（可为空，即便用户被删除了，收藏记录也应该存在）
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// 被收藏的预制菜 ID
    /// </summary>
    public Guid PresetId { get; set; }
    
    /// <summary>
    /// 导航属性：被收藏的预制菜（可为空，即便预制菜被删除了，收藏记录也应该有效，比如显示——"此预制菜已失效"）
    /// </summary>
    public Preset? Preset { get; set; }

    // "CreatedAt" 字段由 ModelBase 自动提供，将用于计算 Trending
}