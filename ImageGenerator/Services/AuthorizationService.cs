using System.Security.Claims;
using ImageGenerator.Database;
using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Helpers;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Services;

/// <summary>
/// 授权服务实现
/// </summary>
public class AuthorizationService(
    IgDbContext context,
    IHttpContextAccessor httpContextAccessor,
    JwtHelper jwtHelper) : IAuthorizationService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _http = httpContextAccessor;
    private readonly JwtHelper _jwtHelper = jwtHelper;

    /// <summary>
    /// 提权操作
    /// </summary>
    public async Task<ChangeRoleResponseDto> PromoteUserAsync(ChangeRoleDto request)
    {
        var currentUserId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var currentUser = await _context.Users!.FindAsync(currentUserId)
            ?? throw new InvalidOperationException("当前用户不存在");

        var targetUser = await _context.Users!.FindAsync(request.UserId)
            ?? throw new InvalidOperationException("目标用户不存在");

        // 验证权限：只能操作比自己角色低的用户
        if (targetUser.Role >= currentUser.Role)
        {
            throw new UnauthorizedAccessException("无权提升该用户的角色");
        }

        // 验证目标角色：不能提升到比自己更高的角色
        if (request.TargetRole >= currentUser.Role)
        {
            throw new UnauthorizedAccessException($"无权将用户提升至 {request.TargetRole} 角色");
        }

        // 验证目标角色必须高于当前角色
        if (request.TargetRole <= targetUser.Role)
        {
            throw new InvalidOperationException("目标角色必须高于用户当前角色");
        }

        var oldRole = targetUser.Role;
        targetUser.Role = request.TargetRole;

        // 创建审计日志
        var log = new RoleLog
        {
            TargetUserId = targetUser.Id,
            OperatorUserId = currentUserId,
            OldRole = (int)oldRole,
            NewRole = (int)request.TargetRole,
            OperationType = "Promote",
            Reason = request.Reason
        };
        _context.RoleLogs.Add(log);

        await _context.SaveChangesAsync();

        // 生成新的 JWT Token，包含更新后的角色
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, targetUser.Id.ToString()),
            new(ClaimTypes.Role, targetUser.Role.ToString())
        };
        var newToken = _jwtHelper.GetJwtToken(claims);

        return new ChangeRoleResponseDto
        {
            UserId = targetUser.Id,
            Username = targetUser.Username,
            OldRole = oldRole,
            NewRole = targetUser.Role,
            ChangedAt = DateTime.UtcNow,
            NewToken = newToken // 返回新的 Token
        };
    }

    /// <summary>
    /// 降权操作
    /// </summary>
    public async Task<ChangeRoleResponseDto> DemoteUserAsync(ChangeRoleDto request)
    {
        var currentUserId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var currentUser = await _context.Users!.FindAsync(currentUserId)
            ?? throw new InvalidOperationException("当前用户不存在");

        var targetUser = await _context.Users!.FindAsync(request.UserId)
            ?? throw new InvalidOperationException("目标用户不存在");

        // 验证权限：只能操作比自己角色低的用户
        if (targetUser.Role >= currentUser.Role)
        {
            throw new UnauthorizedAccessException("无权降低该用户的角色");
        }

        // 验证目标角色必须低于当前角色
        if (request.TargetRole >= targetUser.Role)
        {
            throw new InvalidOperationException("目标角色必须低于用户当前角色");
        }

        var oldRole = targetUser.Role;
        targetUser.Role = request.TargetRole;

        // 创建审计日志
        var log = new RoleLog
        {
            TargetUserId = targetUser.Id,
            OperatorUserId = currentUserId,
            OldRole = (int)oldRole,
            NewRole = (int)request.TargetRole,
            OperationType = "Demote",
            Reason = request.Reason
        };
        _context.RoleLogs.Add(log);

        await _context.SaveChangesAsync();

        // 生成新的 JWT Token，包含更新后的角色
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, targetUser.Id.ToString()),
            new(ClaimTypes.Role, targetUser.Role.ToString())
        };
        var newToken = _jwtHelper.GetJwtToken(claims);
        var expirationTime = DateTime.Now.AddMinutes(30);

        return new ChangeRoleResponseDto
        {
            UserId = targetUser.Id,
            Username = targetUser.Username,
            OldRole = oldRole,
            NewRole = targetUser.Role,
            ChangedAt = DateTime.UtcNow,
            NewToken = newToken,
            TokenExpirationTime = expirationTime
        };
    }

    /// <summary>
    /// 获取当前用户 ID
    /// </summary>
    private Guid? GetCurrentUserId()
    {
        var val = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return Guid.TryParse(val, out var id) ? id : null;
    }
}
