using ImageGenerator.Dtos;
using ImageGenerator.Helpers;
using ImageGenerator.Interface;
using ImageGenerator.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

/// <summary>
/// 授权管理控制器 - 处理用户角色的提权和降权
/// </summary>
[ApiController]
[Route("api/[controller]")]
[RoleAuthorize(UserRole.PowerUser)] // 至少需要 PowerUser 角色才能访问
public class AuthorizationController(IAuthorizationService authorizationService) : ControllerBase
{
    private readonly IAuthorizationService _authorizationService = authorizationService;

    /// <summary>
    /// 提权操作 - 将用户提升到更高的角色
    /// </summary>
    /// <param name="request">提权请求</param>
    /// <returns>变更结果</returns>
    /// <response code="200">提权成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="401">未认证或无权限</response>
    /// <response code="404">用户不存在</response>
    [HttpPost("promote")]
    [ProducesResponseType(typeof(ChangeRoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChangeRoleResponseDto>> PromoteUser([FromBody] ChangeRoleDto request)
    {
        try
        {
            var result = await _authorizationService.PromoteUserAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 降权操作 - 将用户降低到更低的角色
    /// </summary>
    /// <param name="request">降权请求</param>
    /// <returns>变更结果</returns>
    /// <response code="200">降权成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="401">未认证或无权限</response>
    /// <response code="404">用户不存在</response>
    [HttpPost("demote")]
    [ProducesResponseType(typeof(ChangeRoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChangeRoleResponseDto>> DemoteUser([FromBody] ChangeRoleDto request)
    {
        try
        {
            var result = await _authorizationService.DemoteUserAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
