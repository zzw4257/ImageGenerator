using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

/// <summary>
/// 提权/降权请求 DTO
/// </summary>
public class ChangeRoleDto
{
    /// <summary>
    /// 目标用户 ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 目标角色
    /// </summary>
    public UserRole TargetRole { get; set; }

    /// <summary>
    /// 操作原因/备注（可选）
    /// </summary>
    public string? Reason { get; set; }
}

/// <summary>
/// 角色变更响应 DTO
/// </summary>
public class ChangeRoleResponseDto
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 原角色
    /// </summary>
    public UserRole OldRole { get; set; }

    /// <summary>
    /// 新角色
    /// </summary>
    public UserRole NewRole { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// 新的 JWT Token（用户可以立即使用新权限，无需重新登录）
    /// </summary>
    public string NewToken { get; set; } = string.Empty;

    /// <summary>
    /// Token 过期时间
    /// </summary>
    public DateTime TokenExpirationTime { get; set; }
}
