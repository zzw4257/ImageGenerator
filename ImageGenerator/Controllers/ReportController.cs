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

    // (未来可以添加 PUT /api/reports/{reportId}/status 来处理举报)
}