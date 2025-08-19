using Microsoft.AspNetCore.Mvc;
using ImageGenerator.Interface;
using ImageGenerator.Dtos;
using AutoMapper;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitationController(IInvitationService invitationService, IMapper mapper) : ControllerBase
{
    private readonly IInvitationService _invitationService = invitationService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("codes")]
    public async Task<ActionResult<IEnumerable<InvitationDto>>> GetInvitationCodes()
    {
        try
        {
            var codes = await _invitationService.GetInvitationCodeAsync();
            return Ok(_mapper.Map<IEnumerable<InvitationDto>>(codes));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"获取邀请失败: {ex.Message}");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<InvitationDto>> CreateInvitation()
    {
        try
        {
            var invitation = await _invitationService.CreateInvitationAsync();
            return Ok(_mapper.Map<InvitationDto>(invitation));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"创建邀请失败: {ex.Message}");
        }
    }

    [HttpDelete("{invitationId}")]
    public async Task<ActionResult> DeleteInvitation(Guid invitationId)
    {
        try
        {
            await _invitationService.DeleteInvitationAsync(invitationId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return NotFound($"创建邀请失败: {ex.Message}");
        }
    }
}