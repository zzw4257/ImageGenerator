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
    public Preset? Preset { get; set; } //举报成功可能被删除

    /// <summary>
    /// 提交举报的用户 ID
    /// </summary>
    public Guid ReporterUserId { get; set; }
    public User ReporterUser { get; set; } = null!;

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