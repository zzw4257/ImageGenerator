namespace ImageGenerator.Enums;

/// <summary>
/// 用户角色枚举
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 普通用户 - 可以访问除了 ReportController 以外的 Controller
    /// </summary>
    User = 0,

    /// <summary>
    /// 高级用户 - 可以访问除了 ReportController 以外的 Controller
    /// </summary>
    PowerUser = 1,

    /// <summary>
    /// 管理员 - 可以访问所有 Controller
    /// </summary>
    Admin = 2,

    /// <summary>
    /// 所有者 - 可以访问所有服务
    /// </summary>
    Owner = 3
}
