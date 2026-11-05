using hotel_admin_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLite;

namespace hotel_admin_api.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly SQLiteConnection _database;

        public HotelsController(SQLiteConnection database)
        {
            _database = database;
        }

        [HttpGet(Name = "GetHotels")]
        public ActionResult<List<Hotel>> Get(
            [FromQuery] string? name,
            [FromQuery] double? rating)
        {
            if (rating.HasValue && !double.IsFinite(rating.Value))
            {
                return BadRequest("The rating filter must be a finite number.");
            }

            var hotels = _database.Table<Hotel>().ToList();
            if (!string.IsNullOrWhiteSpace(name))
            {
                hotels = hotels
                    .Where(hotel => hotel.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (rating.HasValue)
            {
                hotels = hotels
                    .Where(hotel => hotel.AverageReviewScore >= rating.Value)
                    .ToList();
            }

            return Ok(hotels);
        }

        [HttpGet("{id}", Name = "GetHotelById")]
        public ActionResult<Hotel> GetById(int id)
        {
            var hotel = _database.Find<Hotel>(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }
        [HttpPost(Name = "CreateHotel")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Hotel> Create(Hotel hotel)
        {
            if (hotel == null) return BadRequest();

            hotel.Id = 0;
            _database.Insert(hotel);
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id}", Name = "UpdateHotel")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, Hotel updatedHotel)
        {
            if (updatedHotel == null) return BadRequest();

            updatedHotel.Id = id;
            return _database.Update(updatedHotel) == 0 ? NotFound() : NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteHotel")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            return _database.Delete<Hotel>(id) == 0 ? NotFound() : NoContent();
        }
    }
}