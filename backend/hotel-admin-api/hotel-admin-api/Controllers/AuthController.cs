using hotel_admin_api.DTOs;
using hotel_admin_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace hotel_admin_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var response = _authService.Login(request);
        return response is null ? Unauthorized() : Ok(response);
    }
}
