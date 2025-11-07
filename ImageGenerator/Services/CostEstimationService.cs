// \...\ImageGenerator\ImageGenerator\Services\CostEstimationService.cs
using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Interface;

namespace ImageGenerator.Services;

/// <summary>
/// 预估消耗的服务实现; 
/// </summary>
public class CostEstimationService(CreditCostSettings creditSettings) : ICostEstimationService
{
    // 注入我们之前注册的价目表
    private readonly CreditCostSettings _creditSettings = creditSettings;

    public Task<EstimateResultDto> EstimateCostAsync(EstimateRequestDto request)
    {
        //  构建价目表查询键
        string exactKey = $"{request.Provider}.{request.Capability}";
        string defaultKey = "Default";

        decimal cost;
        string? warning = null;
        string pricingKey = exactKey;

        //  尝试获取精确定价
        if (!_creditSettings.Costs.TryGetValue(exactKey, out cost))
        {
            //  如果失败，尝试获取默认定价
            if (!_creditSettings.Costs.TryGetValue(defaultKey, out cost))
            {
                //  连默认定价都没有，返回 0 并告警（或者抛出异常）
                cost = 0;
                warning = $"未找到键 '{exactKey}' 或 '{defaultKey}' 的定价配置。";
                pricingKey = "Error";
            }
            else
            {
                warning = $"未找到精确定价 '{exactKey}'，已使用默认值。";
                pricingKey = defaultKey;
            }
        }

        //  TODO: 未来在这里添加基于分辨率(request.Width/Height)的额外计费逻辑; 但是具体逻辑可能没有那么简单，暂且按下不表; 目前定价依据为appsettings.json
        // 例如: if (request.Width > 1024) { cost *= 1.5m; }

        //  构造响应
        var result = new EstimateResultDto
        {
            EstimatedCost = cost,
            Warning = warning,
            PricingKey = pricingKey
        };

        return Task.FromResult(result);
    }
}