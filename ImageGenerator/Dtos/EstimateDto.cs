namespace ImageGenerator.Dtos;

/// <summary>
/// 费用估算请求 DTO
/// </summary>
public class EstimateRequestDto
{
    /// <summary>
    /// 预制菜 ID（可选）
    /// </summary>
    public Guid? PresetId { get; set; }

    /// <summary>
    /// 提示词（如果不使用预制菜）
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string Provider { get; set; } = "Stub";

    /// <summary>
    /// 生成参数
    /// </summary>
    public Dictionary<string, object>? Params { get; set; }
}

/// <summary>
/// 费用估算响应 DTO
/// </summary>
public class EstimateResponseDto
{
    /// <summary>
    /// 估算费用（Credits）
    /// </summary>
    public decimal EstimatedCost { get; set; }

    /// <summary>
    /// 供应商
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 预制菜名称（如果使用了预制菜）
    /// </summary>
    public string? PresetName { get; set; }

    /// <summary>
    /// 估算依据说明
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
