using ImageGenerator.Models;

namespace ImageGenerator.Models;

/// <summary>
/// 记录用户和预制菜之间的“点赞”关系。
/// 仅用于防止重复点赞和实现取消点赞。
/// </summary>
public class PresetLike: ModelBase
{
    /// <summary>
    /// 点赞的用户 ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// 导航属性：点赞的用户（可为空，即便用户被删除了，点赞记录也应该存在）
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// 被点赞的预制菜 ID
    /// </summary>
    public Guid PresetId { get; set; }
    
    /// <summary>
    /// 导航属性：被点赞的预制菜（可为空，即便预制菜被删了，这条点赞记录也应该存在）
    /// </summary>
    public Preset? Preset { get; set; }
}