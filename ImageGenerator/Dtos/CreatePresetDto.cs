using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Dtos;

/// <summary>
/// 用于创建新 Preset 时的数据传输对象 (DTO)
/// </summary>
public class CreatePresetDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string CoverUrl { get; set; } = string.Empty;

    [Required]
    public string Prompt { get; set; } = string.Empty;

    [Required]
    public string Provider { get; set; } = "Stub";

    [Required]
    public int PriceCredits { get; set; } = 0;

    public string DefaultParams { get; set; } = "{}";
    
    public List<string> Tags { get; set; } = [];
}