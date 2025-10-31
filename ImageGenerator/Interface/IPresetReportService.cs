// D:\...\ImageGenerator\ImageGenerator\Interface\IPresetReportService.cs

using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// 处理预制菜举报相关操作的服务接口(管理员)
/// </summary>
public interface IPresetReportService
{
    /// <summary>
    /// 异步获取所有举报记录 (管理员权限)。
    /// </summary>
    /// <param name="statusFilter">（可选）按举报状态筛选</param>
    /// <returns>举报记录列表</returns>
    Task<IEnumerable<PresetReportDto>> GetAllReportsAsync(ReportStatus? statusFilter = null);

    // (未来可以添加: Task MarkReportStatusAsync(Guid reportId, ReportStatus newStatus))
}