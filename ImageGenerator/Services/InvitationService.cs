using ImageGenerator.Interface;
using ImageGenerator.Models;
using ImageGenerator.Database;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

public class InvitationService(IgDbContext context, IHttpContextAccessor httpContextAccessor) : IInvitationService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<List<Invitation>> GetInvitationCodeAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未知ID");;
        return await _context.Invitations!
            .Where(i => i.IssuerId == userId)
            .ToListAsync();
    }

    public async Task<Invitation> CreateInvitationAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未知ID");

        var code = GenerateInvitationCode();
        var invitation = new Invitation
        {
            Code = code,
            IssuerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Invitations!.Add(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }
        catch (DbUpdateException)
        {
            throw new ArgumentException("邀请代码已存在，请重试。");
        }
        catch (Exception ex)
        {
            throw new Exception($"创建邀请失败: {ex.Message}");
        }
    }

    private static string GenerateInvitationCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string([.. Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)])]);
    }

    public async Task DeleteInvitationAsync(Guid invitationId)
    {
        var invitation = await _context.Invitations!.FindAsync(invitationId);
        if (invitation == null || invitation.IsDeleted)
        {
            throw new ArgumentException("邀请不存在或已被删除");
        }

        invitation.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}