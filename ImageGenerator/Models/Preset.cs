using System.ComponentModel.DataAnnotations;

namespace ImageGenerator.Models;

/// <summary>
/// 代表一个预配置的“预制菜”(Preset) 模板。
/// 它将提示词、供应商、默认参数和价格打包成一个面向用户的可选项。
/// </summary>
public class Preset : ModelBase
{
    /// <summary>
    /// 预制菜的显示名称 (例如 "产品商业摄影").
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 预制菜的简短描述，说明其用途或效果。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 预制菜的封面图 URL (例如 "/images/presets/product-shot.png").
    /// </summary>
    [Required]
    public string CoverUrl { get; set; } = string.Empty;

    /// <summary>
    /// 完整的、将发送给供应商的原始提示词模板。
    /// </summary>
    [Required]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// 执行此预制菜的默认供应商 (例如 "Stub", "Qwen", "Flux").
    /// </summary>
    [Required]
    public string Provider { get; set; } = "Stub";

    /// <summary>
    /// 使用此预制菜需要消耗的 Credits 点数。
    /// </summary>
    public int PriceCredits { get; set; } = 0;

    /// <summary>
    /// 默认参数的 JSON 字符串 (例如 {"width": 768, "style": "cinematic"}).
    /// </summary>
    public string DefaultParams { get; set; } = "{}";

    /// <summary>
    /// 用于分类和筛选的标签列表 (例如 ["product", "cinematic"])。
    /// 存储为 JSON 字符串数组。
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// 创建此预制菜的用户 ID。
    /// </summary>
    [Required]
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// 导航属性，指向创建此预制菜的用户。
    /// </summary>
    public User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// 关联到使用此预制菜的所有生成记录。
    /// (对应Spec B 节的 GenerationTask.PresetId)
    /// </summary>
    public ICollection<GenerationRecord> GenerationRecords { get; set; } = [];
}