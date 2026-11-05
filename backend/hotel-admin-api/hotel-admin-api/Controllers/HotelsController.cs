using hotel_admin_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_admin_api.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        //Change this to a database connection in the future
        static private List<Hotel> hotels = new List<Hotel>
        {
            new Hotel { Id = 1, Name = "Hotel A", Address = "123 Main St" },
            new Hotel { Id = 2, Name = "Hotel B", Address = "456 Elm St" },
            new Hotel { Id = 3, Name = "Hotel C", Address = "789 Oak St" },
            new Hotel { Id = 4, Name = "Hotel D", Address = "101 Pine St" },
        };

        [HttpGet(Name = "GetHotels")]
        public ActionResult<List<Hotel>> Get()
        {
            return Ok(hotels);
        }

        [HttpGet("{id}", Name = "GetHotelById")]
        public ActionResult<Hotel> GetById(int id)
        {
            var hotel = hotels.FirstOrDefault(hotel => hotel.Id == id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }
        //For this methods, add validation of user authentication and authorization to create, update, or delete.
        [HttpPost(Name = "CreateHotel")]
        public ActionResult<Hotel> Create(Hotel hotel)
        {
            if(hotel == null) return BadRequest();
            hotels.Add(hotel);
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id}", Name = "UpdateHotel")]
        public IActionResult Update(int id, Hotel updatedHotel)
        {
            var hotel = hotels.FirstOrDefault(hotel => hotel.Id == id);
            if (hotel == null) return NotFound();
            hotel.Name = updatedHotel.Name;
            hotel.Address = updatedHotel.Address;
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteHotel")]
        public IActionResult Delete(int id)
        {
            var hotel = hotels.FirstOrDefault(hotel => hotel.Id == id);
            if (hotel == null) return NotFound();
            hotels.Remove(hotel);
            return NoContent();
        }
    }
}