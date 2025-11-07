// D:\...\ImageGenerator\ImageGenerator\Interface\IPresetReportService.cs

using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// 处理预制菜举报相关操作的服务接口(管理�?
/// </summary>
public interface IPresetReportService
{
    /// <summary>
    /// 异步获取所有举报记录，可选按状态筛选
    /// </summary>
    /// <param name="statusFilter">（可选）按举报状态筛选</param>
    /// <returns>举报记录列表</returns>
    Task<IEnumerable<PresetReportDto>> GetAllReportsAsync(ReportStatus? statusFilter = null);

    /// <summary>
    /// 处理举报记录：删除被举报的预制菜或驳回举报
    /// </summary>
    /// <param name="reportId">举报记录 ID</param>
    /// <param name="action">"delete_preset" 或 "dismiss_report"</param>
    /// <returns>处理结果</returns>
    Task<bool> HandleReportAsync(Guid reportId, ReportHandle action);
}

