// D:\...\ImageGenerator\ImageGenerator\Dtos\PresetReportDto.cs
using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

public class PresetReportDto //防止Report和User循环引用而设计 
{
    public Guid ReportId { get; set; } // 举报记录本身的 ID
    public Guid PresetId { get; set; }

    public string PresetNameSnapshot { get; set; } = string.Empty; // 来自 PresetReport 的快照
    public string? PresetCoverUrlSnapshot { get; set; } // 来自 PresetReport 的快照

    public Guid ReporterUserId { get; set; }
    public string ReporterUsername { get; set; } = string.Empty; // 只包含用户名，而不是整个 User 对象

    public ReportReason Reason { get; set; }
    public string? Notes { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}