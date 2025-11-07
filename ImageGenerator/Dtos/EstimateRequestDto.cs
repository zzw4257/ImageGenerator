// \...\ImageGenerator\ImageGenerator\Dtos\EstimateRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Dtos;

/// <summary>
/// 请求实时预估消耗的 DTO
/// </summary>
public class EstimateRequestDto
{
    /// <summary>
    /// 必需：模型供应商 (例如 "Qwen", "Flux", "openai")
    /// </summary>
    [Required]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 必需：能力类型 (例如 "TextToImage", "ImageToImage")
    /// </summary>
    [Required]
    public string Capability { get; set; } = "TextToImage";

    /// <summary>
    /// 可选：用于更精确定价的宽度; 注：图像生成成本可能和尺寸相关
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 可选：用于更精确定价的高度
    /// </summary>
    public int? Height { get; set; }
}