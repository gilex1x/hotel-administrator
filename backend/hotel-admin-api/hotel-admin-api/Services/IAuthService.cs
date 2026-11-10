using hotel_admin_api.DTOs;

namespace hotel_admin_api.Services;

public interface IAuthService
{
    LoginResponse? Login(LoginRequest request);
}
