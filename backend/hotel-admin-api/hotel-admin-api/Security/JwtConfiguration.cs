using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace hotel_admin_api.Security;

public static class JwtConfiguration
{
    public const string Issuer = "hotel-admin-api";
    public const string Audience = "hotel-admin-api";

    public static SymmetricSecurityKey GetSigningKey(IConfiguration configuration)
    {
        var configuredKey = configuration["Authentication:SigningKey"];
        if (string.IsNullOrWhiteSpace(configuredKey) || Encoding.UTF8.GetByteCount(configuredKey) < 32)
        {
            throw new InvalidOperationException(
                "Authentication:SigningKey must be configured and contain at least 32 bytes.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredKey));
    }
}
