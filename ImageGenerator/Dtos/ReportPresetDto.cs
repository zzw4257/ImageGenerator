// D:\...\ImageGenerator\ImageGenerator\Dtos\ReportPresetDto.cs

using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

/// <summary>
/// 用于举报 Preset 时的数据传输对象 (DTO)
/// </summary>
public class ReportPresetDto
{
    /// <summary>
    /// 举报原因
    /// </summary>
    public ReportReason Reason { get; set; }

    /// <summary>
    /// (可选) 补充说明
    /// </summary>
    public string? Notes { get; set; }
}