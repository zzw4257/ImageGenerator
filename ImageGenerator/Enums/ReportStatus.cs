namespace ImageGenerator.Enums;

/// <summary>
/// 举报的处理状态
/// </summary>
public enum ReportStatus
{
    Pending,   // 待处理
    Reviewed,  // 审核中
    Resolved,  // 已处理
    Dismissed  // 已驳回
}