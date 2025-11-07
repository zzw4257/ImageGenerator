using ImageGenerator.Models;
using ImageGenerator.Dtos;

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

    /// <summary>
    /// 异步根据 ID 获取单个预制菜。
    /// </summary>
    /// <param name="id">预制菜的 Guid ID</param>
    /// <returns>单个 Preset 对象，如果未找到则返回 null</returns>
    Task<Preset?> GetPresetByIdAsync(Guid id);

    /// <summary>
    /// 异步创建一个新的预制菜。
    /// </summary>
    /// <param name="dto">创建 Preset 所需的数据</param>
    /// <returns>已创建并存入数据库的 Preset 对象</returns>
    Task<Preset> CreatePresetAsync(CreatePresetDto dto);

    /// <summary>
    /// 异步软删除一个预制菜。
    /// </summary>
    /// <param name="id">要删除的预制菜 ID</param>
    /// <returns>操作是否成功 (true: 找到并标记删除, false: 未找到)</returns>
    Task<bool> DeletePresetAsync(Guid id);

    /// <summary>
    /// 异步获取当前登录用户创建的所有预制菜。
    /// </summary>
    /// <returns>当前用户创建的 Preset 列表</returns>
    Task<IEnumerable<Preset>> GetMyPresetsAsync();
}