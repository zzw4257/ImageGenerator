namespace ImageGenerator.Models;

/// <summary>
/// 角色变更审计日志
/// </summary>
public class RoleLog : ModelBase
{
    /// <summary>
    /// 目标用户 ID（被提权/降权的用户）
    /// </summary>
    public Guid TargetUserId { get; set; }
    
    /// <summary>
    /// 目标用户（导航属性）
    /// </summary>
    public User? TargetUser { get; set; }

    /// <summary>
    /// 操作者用户 ID（执行提权/降权的管理员）
    /// </summary>
    public Guid OperatorUserId { get; set; }
    
    /// <summary>
    /// 操作者用户（导航属性）
    /// </summary>
    public User? OperatorUser { get; set; }

    /// <summary>
    /// 原角色
    /// </summary>
    public int OldRole { get; set; }

    /// <summary>
    /// 新角色
    /// </summary>
    public int NewRole { get; set; }

    /// <summary>
    /// 操作类型（Promote 或 Demote）
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 操作原因/备注（可选）
    /// </summary>
    public string? Reason { get; set; }
}
