using ImageGenerator.Enums; 
using ImageGenerator.Models;
using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Models;

/// <summary>
/// 记录用户对预制菜的“举报”事件。
/// </summary>
public class PresetReport : ModelBase
{
    /// <summary>
    /// 被举报的预制菜 ID
    /// </summary>
    public Guid PresetId { get; set; }
    
    /// <summary>
    /// 导航属性：被举报的预制菜（可为空，Preset 被删除后举报记录仍需保留用于审计）
    /// </summary>
    public Preset? Preset { get; set; } 
    /// <summary>
    /// 提交举报的用户 ID
    /// </summary>
    public Guid ReporterUserId { get; set; }
    
    /// <summary>
    /// 导航属性：提交举报的用户（可为空，即便用户被删除了，举报记录也应该存在用于审计）
    /// </summary>
    public User? ReporterUser { get; set; }

    /// <summary>
    /// 举报原因 
    /// </summary>
    public ReportReason Reason { get; set; }

    /// <summary>
    /// 举报的补充说明
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 此举报的处理状态 (Pending, Resolved, etc.)
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    /// <summary>
    /// (快照) 被举报时，预制菜的名称。
    /// 用于在原始 Preset 被删除后仍能显示。
    /// </summary>
    [Required] // 我们需要至少知道名字
    public string PresetNameSnapshot { get; set; } = string.Empty;

    /// <summary>
    /// (快照, 可选) 被举报时，预制菜的描述。
    /// </summary>
    public string? PresetDescriptionSnapshot { get; set; }

    /// <summary>
    /// (快照, 可选) 被举报时，预制菜的封面图 URL。
    /// </summary>
    public string? PresetCoverUrlSnapshot { get; set; }

    // "CreatedAt" 字段由 ModelBase 自动提供
}