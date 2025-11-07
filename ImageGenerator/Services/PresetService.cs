using AutoMapper;
using ImageGenerator.Database; 
using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using ImageGenerator.Helpers;
using Microsoft.EntityFrameworkCore; 
using System.Security.Claims;

namespace ImageGenerator.Services;

/// <summary>
/// 预制菜(Preset)相关操作的实现
/// </summary>
public class PresetService(IgDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IPresetService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// 从 HttpContext 中安全地获取当前登录用户的 ID。
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Name); //根据AuthenticationService.cs; ClaimTypes.Name对应user.Id.toString()
            
        if (Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        
        throw new UnauthorizedAccessException("无法识别用户身份。");
    }

    /// <summary>
    /// 异步获取所有可用的预制菜列表。
    /// </summary>
    /// <returns>预制菜模型列表</returns>
    public async Task<PagedList<Preset, PresetDto>> GetPresetsAsync(PaginationBaseDto param)
    {
        var presents =  _context.Presets!
            .Include(p => p.CreatedByUser)
            .OrderBy(p => p.CreatedAt);
        return await PagedList<Preset, PresetDto>.CreateAsync(presents.AsQueryable(), param, _mapper);
    }

    /// <summary>
    /// 异步根据 ID 获取单个预制菜。
    /// </summary>
    public async Task<PresetDetailedDto?> GetPresetByIdAsync(Guid id)
    {
        var present = await _context.Presets!
            .Include(p => p.CreatedByUser)
            .Include(p => p.PresetFavorites)
            .Include(p => p.PresetLikes)
            .FirstOrDefaultAsync(p => p.Id == id);
        return _mapper.Map<PresetDetailedDto>(present);
    }

    /// <summary>
    /// 异步创建一个新的预制菜。
    /// </summary>
    public async Task<PresetDetailedDto> CreatePresetAsync(CreatePresetDto dto)
    {
        var currentUserId = GetCurrentUserId();
        var newPreset = new Preset
        {
            Name = dto.Name,
            Description = dto.Description,
            CoverUrl = dto.CoverUrl,
            Prompt = dto.Prompt,
            Provider = dto.Provider,
            PriceCredits = dto.PriceCredits,
            DefaultParams = dto.DefaultParams,
            Tags = dto.Tags,
            CreatedByUserId = currentUserId
            // Id, CreatedAt, IsDeleted 会由 ModelBase 自动处理
        };

        // 添加到数据库并保存
        await _context.Presets!.AddAsync(newPreset);
        await _context.SaveChangesAsync();

        return _mapper.Map<PresetDetailedDto>(newPreset);
    }

    /// <summary>
    /// 异步软删除一个预制菜。
    /// </summary>
    public async Task<bool> DeletePresetAsync(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var preset = await _context.Presets!
            .FirstOrDefaultAsync(p => p.Id == id);
        if (preset == null)
        {
            return false;
        }
        if (preset.CreatedByUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("您只能删除自己创建的预制菜。");
        }
        preset.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 异步获取当前登录用户创建的所有预制菜。
    /// </summary>
    public async Task<IEnumerable<PresetDto>> GetMyPresetsAsync()
    {
        var currentUserId = GetCurrentUserId();

        //    和 GetPresetsAsync() 几乎一样，但多了一个 WHERE 条件
        var presets = await _context.Presets!
            .Where(p => !p.IsDeleted && p.CreatedByUserId == currentUserId)
            .OrderByDescending(p => p.CreatedAt) // 按最新创建的排序
            .AsNoTracking()
            .ToListAsync();
            
        return _mapper.Map<IEnumerable<PresetDto>>(presets);
    }

}