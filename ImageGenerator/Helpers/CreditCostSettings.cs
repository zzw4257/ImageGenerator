// \...\ImageGenerator\ImageGenerator\Helpers\CreditCostSettings.cs

namespace ImageGenerator.Helpers;

/// <summary>
/// 绑定 appsettings.json 中的 "CreditCosts" 部分。
/// 允许通过 DI 注入价格表。
/// </summary>
public class CreditCostSettings
{
    /// <summary>
    /// 存储所有价格映射，键（例如 "Qwen.TextToImage"）和值（消耗的 Credits）。
    /// </summary>
    public Dictionary<string, decimal> Costs { get; set; } = new();
}