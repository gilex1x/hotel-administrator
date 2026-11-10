using hotel_admin_api.Models;

namespace hotel_admin_api.Repositories;

public interface IHotelRepository
{
    IReadOnlyList<Hotel> GetAll();
    Hotel? GetById(int id);
    Hotel Add(Hotel hotel);
    bool Update(Hotel hotel);
    bool Delete(int id);
}
