using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using hotel_admin_api.Models;
using hotel_admin_api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SQLite;

namespace hotel_admin_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SQLiteConnection _database;
    private readonly SymmetricSecurityKey _signingKey;

    public AuthController(SQLiteConnection database, IConfiguration configuration)
    {
        _database = database;
        _signingKey = JwtConfiguration.GetSigningKey(configuration);
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest();
        }

        var user = _database.Table<User>().FirstOrDefault(candidate => candidate.Username == request.Username);
        if (user is null || !PasswordHashing.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: JwtConfiguration.Issuer,
            audience: JwtConfiguration.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            ],
            notBefore: now,
            expires: now.AddHours(1),
            signingCredentials: credentials);

        return Ok(new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            token.ValidTo));
    }
}

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
