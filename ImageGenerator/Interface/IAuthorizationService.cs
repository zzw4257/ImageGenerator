using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

/// <summary>
/// 授权服务接口
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// 提权操作
    /// </summary>
    /// <param name="request">提权请求</param>
    /// <returns>变更结果</returns>
    Task<ChangeRoleResponseDto> PromoteUserAsync(ChangeRoleDto request);

    /// <summary>
    /// 降权操作
    /// </summary>
    /// <param name="request">降权请求</param>
    /// <returns>变更结果</returns>
    Task<ChangeRoleResponseDto> DemoteUserAsync(ChangeRoleDto request);
}
