using Microsoft.AspNetCore.Mvc;
using ExamenII_PrograWeb.DTOs;
using ExamenII_PrograWeb.Services;

namespace ExamenII_PrograWeb.Controllers;

[ApiController]

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
      
        [FromBody] RegisterRequestDto dto)
    {
        try
        {
            var user = await _authService.Register(dto);
            
            return Ok(new { user.Id, user.Email});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto dto)
    {
        try
        {
            var token = await _authService.Login(dto);
            
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}