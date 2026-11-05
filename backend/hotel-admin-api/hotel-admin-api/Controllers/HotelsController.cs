using hotel_admin_api.DTOs;
using hotel_admin_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotel_admin_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet(Name = "GetHotels")]
    public ActionResult<IReadOnlyList<HotelDto>> Get([FromQuery] HotelListQuery query)
    {
        return Ok(_hotelService.GetHotels(query));
    }

    [HttpGet("{id}", Name = "GetHotelById")]
    public ActionResult<HotelDto> GetById(int id)
    {
        var hotel = _hotelService.GetHotelById(id);
        return hotel is null ? NotFound() : Ok(hotel);
    }

    [HttpPost(Name = "CreateHotel")]
    [Authorize(Roles = "Admin")]
    public ActionResult<HotelDto> Create([FromBody] HotelWriteRequest request)
    {
        var hotel = _hotelService.CreateHotel(request);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id}", Name = "UpdateHotel")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, [FromBody] HotelWriteRequest request)
    {
        return _hotelService.UpdateHotel(id, request) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}", Name = "DeleteHotel")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        return _hotelService.DeleteHotel(id) ? NoContent() : NotFound();
    }
}