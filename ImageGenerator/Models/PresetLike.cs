using ImageGenerator.Models;

namespace ImageGenerator.Models;

/// <summary>
/// 记录用户和预制菜之间的“点赞”关系。
/// 仅用于防止重复点赞和实现取消点赞。
/// </summary>
public class PresetLike  //无继承，节省空间
{
    /// <summary>
    /// 点赞的用户 ID
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>
    /// 被点赞的预制菜 ID
    /// </summary>
    public Guid PresetId { get; set; }
    public Preset? Preset { get; set; } //可为空，即便预制菜被删了，这条点赞记录也应该存在
}