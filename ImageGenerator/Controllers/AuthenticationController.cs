using Microsoft.AspNetCore.Mvc;
using ImageGenerator.Interface;
using ImageGenerator.Dtos;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Handles user authentication, including login and registration.
/// </summary>
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    /// <summary>
    /// Authenticates a user and returns a login DTO with a JWT token.
    /// </summary>
    /// <param name="loginFormDto">The login form data containing the username and password.</param>
    /// <returns>An <see cref="ActionResult"/> containing the <see cref="LoginDto"/>.</returns>
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

    /// <summary>
    /// Registers a new user and returns a login DTO with a JWT token.
    /// </summary>
    /// <param name="registerFormDto">The registration form data containing the username, password, and invitation code.</param>
    /// <returns>An <see cref="ActionResult"/> containing the <see cref="LoginDto"/>.</returns>
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
