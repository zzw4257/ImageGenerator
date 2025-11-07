// D:\...\ImageGenerator\ImageGenerator\Services\PresetReportService.cs
using ImageGenerator.Dtos;
using ImageGenerator.Database;
using ImageGenerator.Enums;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

/// <summary>
/// 处理预制菜举报相关操作的服务实现(管理员)
/// </summary>
public class PresetReportService(IgDbContext context) : IPresetReportService
{
    private readonly IgDbContext _context = context;

    /// <summary>
    /// 异步获取所有举报记录 (管理员权限)。
    /// </summary>
    public async Task<IEnumerable<PresetReportDto>> GetAllReportsAsync(ReportStatus? statusFilter = null)
    {
        IQueryable<PresetReport> query = _context.PresetReports
            .AsNoTracking();

        if (statusFilter.HasValue)
        {
            query = query.Where(r => r.Status == statusFilter.Value);
        }

        query = query.OrderByDescending(r => r.CreatedAt);

        // 使用 .Select() 进行映射，EF Core 会自动进行 JOIN
        return await query.Select(r => new PresetReportDto
        {
            ReportId = r.Id,
            PresetId = r.PresetId,
            PresetNameSnapshot = r.PresetNameSnapshot,
            PresetCoverUrlSnapshot = r.PresetCoverUrlSnapshot,
            ReporterUserId = r.ReporterUserId,
            ReporterUsername = r.ReporterUser != null ? r.ReporterUser.Username : "[已删除]", // 处理 User 可能被删除的情况
            Reason = r.Reason,
            Notes = r.Notes,
            Status = r.Status,
            CreatedAt = r.CreatedAt
        }).ToListAsync();
    }

    /// <summary>
    /// 处理举报记录：删除被举报的预制菜或驳回举报
    /// </summary>
    public async Task<bool> HandleReportAsync(Guid reportId, ReportHandle action)
    {
        var report = await _context.PresetReports
            .Include(r => r.Preset)
            .FirstOrDefaultAsync(r => r.Id == reportId && !r.IsDeleted)
            ?? throw new InvalidOperationException("举报记录不存在");

        if (report.Status != ReportStatus.Pending)
        {
            throw new InvalidOperationException("只能处理待处理状态的举报");
        }

        switch (action)
        {
            case ReportHandle.Delete:
                // 删除被举报的预制菜
                if (report.Preset != null)
                {
                    report.Preset.IsDeleted = true;
                    report.Status = ReportStatus.Resolved;
                }
                else
                {
                    throw new InvalidOperationException("被举报的预制菜已不存在");
                }
                break;

            case ReportHandle.Dismiss:
                // 驳回举报
                report.Status = ReportStatus.Dismissed;
                break;

            default:
                throw new ArgumentException($"无效的操作类型");
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
