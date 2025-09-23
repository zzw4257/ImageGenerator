using Microsoft.AspNetCore.Mvc;
using ImageGenerator.Interface;
using ImageGenerator.Dtos;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginDto>> Login([FromBody] LoginFormDto loginFormDto)
    {
        try
        {
            var result = await _authenticationService.LoginAsync(loginFormDto.Username, loginFormDto.Password);
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginDto>> Register([FromBody] RegisterFormDto registerFormDto)
    {
        try
        {
            var result = await _authenticationService.RegisterAsync(registerFormDto.Username, registerFormDto.Password, registerFormDto.InvitationCode);
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
