// \...\ImageGenerator\ImageGenerator\Dtos\EstimateResultDto.cs

namespace ImageGenerator.Dtos;

/// <summary>
/// 返回实时预估消耗的 DTO
/// </summary>
public class EstimateResultDto
{
    /// <summary>
    /// 预估消耗的 Credits
    /// </summary>
    public decimal EstimatedCost { get; set; }

    /// <summary>
    /// 计费的货币单位
    /// </summary>
    public string Currency { get; set; } = "Credits";

    /// <summary>
    /// 计费所依据的键
    /// </summary>
    public string PricingKey { get; set; } = string.Empty;

    /// <summary>
    /// （可选）关于定价的警告信息，例如"未找到精确定价，已使用默认值"
    /// </summary>
    public string? Warning { get; set; }
}