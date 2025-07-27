using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtService;

    public AuthController(JwtTokenService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        // For demo: Hardcoded user check
        if (login.Username == "admin" && login.Password == "password")
        {
            var token = _jwtService.GenerateToken(login.Username, "Admin");
            return Ok(new { token });
        }
        return Unauthorized("Invalid credentials");
    }
}

public class LoginModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
