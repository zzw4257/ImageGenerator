using System.Security.Claims;
using ImageGenerator.Database;
using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

public class ProfileService(IgDbContext context, IHttpContextAccessor httpContextAccessor) : IProfileService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _http = httpContextAccessor;

    public async Task<ProfileDto> GetProfileAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new InvalidOperationException("用户不存在");

        return new ProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            Credits = user.Credits,
            LastCreditClaimedAt = user.LastCreditClaimedAt
        };
    }

    public async Task<ProfileDto> ClaimDailyCreditsAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new InvalidOperationException("用户不存在");

        var now = DateTime.UtcNow;
        if (user.LastCreditClaimedAt.HasValue && user.LastCreditClaimedAt.Value.Date == now.Date)
        {
            throw new InvalidOperationException("今日已领取免费 credits");
        }

        const int grant = 30;
        user.Credits += grant;
        user.LastCreditClaimedAt = now;
        await _context.SaveChangesAsync();

        return new ProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            Credits = user.Credits,
            LastCreditClaimedAt = user.LastCreditClaimedAt
        };
    }

    private Guid? GetCurrentUserId()
    {
        var val = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return Guid.TryParse(val, out var id) ? id : null;
    }
}
