using hotel_admin_api.DTOs;
using hotel_admin_api.Models;
using hotel_admin_api.Repositories;

namespace hotel_admin_api.Services;

public sealed class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;

    public HotelService(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public IReadOnlyList<HotelDto> GetHotels(HotelListQuery query)
    {
        IEnumerable<Hotel> hotels = _hotelRepository.GetAll();
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            hotels = hotels.Where(hotel =>
                hotel.Name.Contains(query.Name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (query.Rating.HasValue)
        {
            hotels = hotels.Where(hotel => hotel.AverageReviewScore >= query.Rating.Value);
        }

        return hotels.Select(ToDto).ToList();
    }

    public HotelDto? GetHotelById(int id)
    {
        var hotel = _hotelRepository.GetById(id);
        return hotel is null ? null : ToDto(hotel);
    }

    public byte[]? GetHotelImage(int id)
    {
        var imageDataUri = _hotelRepository.GetById(id)?.ImageDataUri;
        if (imageDataUri is null)
        {
            return null;
        }

        const string prefix = "data:image/jpeg;base64,";
        return Convert.FromBase64String(imageDataUri[prefix.Length..]);
    }

    public HotelDto CreateHotel(HotelWriteRequest request)
    {
        var hotel = new Hotel
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            Reviews = request.Reviews.ToList(),
            ImageDataUri = request.ImageDataUri
        };

        return ToDto(_hotelRepository.Add(hotel));
    }

    public bool UpdateHotel(int id, HotelWriteRequest request)
    {
        var hotel = _hotelRepository.GetById(id);
        if (hotel is null)
        {
            return false;
        }

        hotel.Name = request.Name.Trim();
        hotel.Address = request.Address.Trim();
        hotel.Reviews = request.Reviews.ToList();
        if (request.ImageDataUri is not null)
        {
            hotel.ImageDataUri = request.ImageDataUri;
        }
        else if (request.RemoveImage)
        {
            hotel.ImageDataUri = null;
        }
        return _hotelRepository.Update(hotel);
    }

    public bool DeleteHotel(int id)
    {
        return _hotelRepository.Delete(id);
    }

    private static HotelDto ToDto(Hotel hotel)
    {
        return new HotelDto(
            hotel.Id,
            hotel.Name,
            hotel.Address,
            hotel.Reviews.ToArray(),
            hotel.AverageReviewScore,
            hotel.ImageDataUri is null ? null : $"/api/hotels/{hotel.Id}/image");
    }
}
