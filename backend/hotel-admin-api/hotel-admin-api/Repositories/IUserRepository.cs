using hotel_admin_api.Models;

namespace hotel_admin_api.Repositories;

public interface IUserRepository
{
    User? FindByUsername(string username);
}
