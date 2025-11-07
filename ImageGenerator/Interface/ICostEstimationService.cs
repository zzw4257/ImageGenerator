// \...\ImageGenerator\ImageGenerator\Interface\ICostEstimationService.cs
using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

/// <summary>
/// 负责根据模型和参数实时预估 Credits 消耗
/// </summary>
public interface ICostEstimationService
{
    /// <summary>
    /// 根据请求参数计算预估的 Credits 消耗
    /// </summary>
    /// <param name="request">包含 Provider, Capability, 分辨率等</param>
    /// <returns>预估结果 DTO</returns>
    Task<EstimateResultDto> EstimateCostAsync(EstimateRequestDto request);
}