using ImageGenerator.Database; 
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore; 

namespace ImageGenerator.Services;

/// <summary>
/// 预制菜(Preset)相关操作的实现
/// </summary>
public class PresetService(IgDbContext context) : IPresetService
{
    private readonly IgDbContext _context = context;

    /// <summary>
    /// 异步获取所有可用的预制菜列表。
    /// </summary>
    /// <returns>预制菜模型列表</returns>
    public async Task<IEnumerable<Preset>> GetPresetsAsync()
    {
        // 从 IgDbContext.Presets 表中获取数据
        // 注意: 你的 DbContext.cs 中 DbSet 是可空的 (Presets { get; set; })
        // 所以我们使用 ! (null-forgiving operator) 来匹配 AuthenticationService 的风格
        return await _context.Presets!
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}