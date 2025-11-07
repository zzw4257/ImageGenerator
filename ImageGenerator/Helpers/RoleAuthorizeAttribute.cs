using ImageGenerator.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ImageGenerator.Helpers;

/// <summary>
/// 基于角色的授权属性
/// </summary>
public class RoleAuthorizeAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 构造函数，指定允许的最小角色级别
    /// </summary>
    /// <param name="minimumRole">允许的最小角色</param>
    public RoleAuthorizeAttribute(UserRole minimumRole)
    {
        // 构建允许的角色列表（包含该角色及更高级别的角色）
        var allowedRoles = new List<string>();
        
        for (int i = (int)minimumRole; i <= (int)UserRole.Owner; i++)
        {
            allowedRoles.Add(((UserRole)i).ToString());
        }
        
        Roles = string.Join(",", allowedRoles);
    }
}
