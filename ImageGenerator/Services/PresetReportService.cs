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
}