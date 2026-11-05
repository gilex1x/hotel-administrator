using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using hotel_admin_api.DTOs;
using hotel_admin_api.Repositories;
using hotel_admin_api.Security;
using Microsoft.IdentityModel.Tokens;

namespace hotel_admin_api.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly SymmetricSecurityKey _signingKey;

    public AuthService(IUserRepository userRepository, SymmetricSecurityKey signingKey)
    {
        _userRepository = userRepository;
        _signingKey = signingKey;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        var user = _userRepository.FindByUsername(request.Username);
        if (user is null || !PasswordHashing.Verify(request.Password, user.PasswordHash))
        {
            return null;
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

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            token.ValidTo);
    }
}
