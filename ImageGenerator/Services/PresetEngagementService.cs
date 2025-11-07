// D:\...\ImageGenerator\ImageGenerator\Services\PresetEngagementService.cs

using ImageGenerator.Database;
using ImageGenerator.Enums;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ImageGenerator.Services;

/// <summary>
/// 处理用户与预制菜互动（点赞、收藏、举报）的服务实现
/// </summary>
public class PresetEngagementService(IgDbContext context, IHttpContextAccessor httpContextAccessor) : IPresetEngagementService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private Guid GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        if (Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("无法识别用户身份。");
    }

    // --- 点赞/取消点赞 ---
    public async Task<bool> LikeAsync(Guid presetId)
    {
        var userId = GetCurrentUserId();

        // 检查 Preset 是否存在且未被删除
        var presetExists = await _context.Presets!
            .AnyAsync(p => p.Id == presetId && !p.IsDeleted);
        if (!presetExists) return false; // 或者抛出 NotFound 异常

        // 尝试插入 PresetLike 记录
        var like = new PresetLike { UserId = userId, PresetId = presetId };
        _context.PresetLikes.Add(like);

        try
        {
            await _context.SaveChangesAsync();

            // 如果插入成功，则更新 Preset 的 LikeCount (使用 SQL 更新以避免并发问题)
            await _context.Presets!
                .Where(p => p.Id == presetId)
                .ExecuteUpdateAsync(updates => updates.SetProperty(p => p.LikeCount, p => p.LikeCount + 1));

            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.Sqlite.SqliteException { SqliteErrorCode: 19 }) // 主键冲突 (已点赞)
        {
            _context.Entry(like).State = EntityState.Detached; // 撤销失败的 Add 操作
            return false; // 返回 false 表示已点赞
        }
    }

    public async Task<bool> UnlikeAsync(Guid presetId)
    {
        var userId = GetCurrentUserId();

        // 尝试删除 PresetLike 记录 (使用 ExecuteDeleteAsync 以获得更好的性能和并发处理)
        var rowsAffected = await _context.PresetLikes
            .Where(l => l.UserId == userId && l.PresetId == presetId)
            .ExecuteDeleteAsync();

        if (rowsAffected > 0)
        {
            // 如果删除成功，则更新 Preset 的 LikeCount
            await _context.Presets!
                .Where(p => p.Id == presetId)
                .ExecuteUpdateAsync(updates => updates.SetProperty(p => p.LikeCount, p => p.LikeCount - 1));
            return true;
        }

        return false; // 返回 false 表示之前未点赞
    }

    public async Task<bool> HasLikedAsync(Guid presetId)
    {
        var userId = GetCurrentUserId();
        // 直接在 PresetLikes 表中检查是否存在记录
        return await _context.PresetLikes
            .AnyAsync(l => l.UserId == userId && l.PresetId == presetId);
    }

    // --- 收藏/取消收藏 ---
    public async Task<bool> FavoriteAsync(Guid presetId)
    {
        var userId = GetCurrentUserId();

        var presetExists = await _context.Presets!.AnyAsync(p => p.Id == presetId && !p.IsDeleted);
        if (!presetExists) return false;

        // 检查是否已收藏
        var alreadyFavorited = await _context.PresetFavorites
            .AnyAsync(f => f.UserId == userId && f.PresetId == presetId && !f.IsDeleted);
        if (alreadyFavorited) return false;

        // 创建收藏记录
        var favorite = new PresetFavorite { UserId = userId, PresetId = presetId };
        _context.PresetFavorites.Add(favorite);

        // 更新 Preset 的 FavoriteCount
        await _context.Presets!
            .Where(p => p.Id == presetId)
            .ExecuteUpdateAsync(updates => updates.SetProperty(p => p.FavoriteCount, p => p.FavoriteCount + 1));

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnfavoriteAsync(Guid presetId)
    {
        var userId = GetCurrentUserId();

        // 查找收藏记录
        var favorite = await _context.PresetFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.PresetId == presetId && !f.IsDeleted);

        if (favorite != null)
        {
            favorite.IsDeleted = true;

            // 更新 Preset 的 FavoriteCount
            await _context.Presets!
                .Where(p => p.Id == presetId)
                .ExecuteUpdateAsync(updates => updates.SetProperty(p => p.FavoriteCount, p => p.FavoriteCount - 1));

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> HasFavoritedAsync(Guid presetId)
    {
        var userId = GetCurrentUserId();
        // 在 PresetFavorites 表中检查是否存在未被软删除的记录
        return await _context.PresetFavorites
            .AnyAsync(f => f.UserId == userId && f.PresetId == presetId && !f.IsDeleted);
    }

    // --- 举报 ---
    public async Task<bool> ReportAsync(Guid presetId, ReportReason reason, string? notes)
    {
        var userId = GetCurrentUserId();

        // 查找被举报的 Preset (需要加载它来填充快照)
        var presetToReport = await _context.Presets!
            .AsNoTracking() // 只读
            .FirstOrDefaultAsync(p => p.Id == presetId && !p.IsDeleted);

        if (presetToReport == null)
        {
            // 不能举报一个不存在或已被删除的 Preset
            return false;
        }

        // 创建举报记录，并填充快照
        var report = new PresetReport
        {
            PresetId = presetId,
            ReporterUserId = userId,
            Reason = reason,
            Notes = notes,
            // --- 填充快照 ---
            PresetNameSnapshot = presetToReport.Name,
            PresetDescriptionSnapshot = presetToReport.Description,
            PresetCoverUrlSnapshot = presetToReport.CoverUrl
        };

        _context.PresetReports.Add(report);
        await _context.SaveChangesAsync();
        return true;
    }
}