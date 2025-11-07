// \...\ImageGenerator\ImageGenerator\Controllers\CostController.cs
using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

/// <summary>
/// 提供与计费和价格预估相关的 API
/// </summary>
[ApiController]
[Route("api/cost")]
[Authorize] // 预估价格也要求用户登录
public class CostController(ICostEstimationService costService) : ControllerBase
{
    private readonly ICostEstimationService _costService = costService;

    /// <summary>
    /// 实时预估生成任务所需的 Credits 消耗
    /// </summary>
    /// <param name="request">包含 Provider, Capability, 分辨率等</param>
    /// <returns>预估的消耗结果</returns>
    [HttpPost("estimate")]
    public async Task<ActionResult<EstimateResultDto>> GetEstimate([FromBody] EstimateRequestDto request)
    {
        var result = await _costService.EstimateCostAsync(request);
        
        if (result.PricingKey == "Error")
        {
            // 如果连默认定价都找不到，这是一个服务器内部配置错误
            return StatusCode(500, result);
        }

        return Ok(result);
    }
}