using ImageGenerator.Models;

namespace ImageGenerator.Interface;

/// <summary>
/// 预制菜(Preset)相关操作的服务接口
/// </summary>
public interface IPresetService
{
    /// <summary>
    /// 异步获取所有可用的预制菜列表。
    /// </summary>
    /// <returns>预制菜模型列表</returns>
    Task<IEnumerable<Preset>> GetPresetsAsync();
}