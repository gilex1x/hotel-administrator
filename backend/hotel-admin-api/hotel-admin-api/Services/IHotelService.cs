using hotel_admin_api.DTOs;

namespace hotel_admin_api.Services;

public interface IHotelService
{
    IReadOnlyList<HotelDto> GetHotels(HotelListQuery query);
    HotelDto? GetHotelById(int id);
    byte[]? GetHotelImage(int id);
    HotelDto CreateHotel(HotelWriteRequest request);
    bool UpdateHotel(int id, HotelWriteRequest request);
    bool DeleteHotel(int id);
}
