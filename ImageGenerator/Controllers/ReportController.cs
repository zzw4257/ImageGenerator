// D:\...\ImageGenerator\ImageGenerator\Controllers\ReportController.cs

using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Helpers;
using ImageGenerator.Interface;
using ImageGenerator.Models; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

/// <summary>
/// 管理举报记录的 API 端点 (管理员权限)
/// </summary>
[ApiController]
[Route("api/reports")] 
[RoleAuthorize(UserRole.Admin)]  // 只允许 Admin 和 Owner 访问
public class ReportController(IPresetReportService reportService) : ControllerBase
{
    private readonly IPresetReportService _reportService = reportService;

    /// <summary>
    /// 获取所有预制菜举报记录
    /// </summary>
    /// <param name="status">（可选）按状态筛选 (Pending, Resolved等)</param>
    /// <returns>200 OK + 举报列表</returns>
    [HttpGet] // 路由: GET /api/reports?status=Pending
    public async Task<ActionResult<IEnumerable<PresetReportDto>>> GetAllReports([FromQuery] ReportStatus? status = null)
    {
        var reports = await _reportService.GetAllReportsAsync(status);
        return Ok(reports);
    }

    /// <summary>
    /// 处理举报记录
    /// </summary>
    /// <param name="reportId">举报记录 ID</param>
    /// <param name="action">操作类型："delete_preset" 或 "dismiss_report"</param>
    /// <returns>处理结果</returns>
    [HttpPost("{reportId}/handle")]
    public async Task<ActionResult> HandleReport(Guid reportId, [FromQuery] ReportHandle action)
    {
        try
        {
            await _reportService.HandleReportAsync(reportId, action);
            return Ok(new { message = "举报处理成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"处理失败: {ex.Message}" });
        }
    }
}
