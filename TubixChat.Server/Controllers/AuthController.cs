using Microsoft.AspNetCore.Mvc;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;

namespace TubixChat.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (result.Success)
            return Ok(new { success = true, message = result.Message, user = result.User });

        return BadRequest(new { success = false, message = result.Message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result.Success)
            return Ok(new { success = true, message = result.Message, user = result.User });

        return BadRequest(new { success = false, message = result.Message });
    }
}