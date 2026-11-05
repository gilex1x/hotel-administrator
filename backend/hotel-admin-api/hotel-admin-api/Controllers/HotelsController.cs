using hotel_admin_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_admin_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        static private List<Hotel> hotels = new List<Hotel>
        {
            new Hotel { Id = 1, Name = "Hotel A", Address = "123 Main St" },
            new Hotel { Id = 2, Name = "Hotel B", Address = "456 Elm St" },
            new Hotel { Id = 3, Name = "Hotel C", Address = "789 Oak St" },
            new Hotel { Id = 4, Name = "Hotel D", Address = "101 Pine St" },
        };
    }
}
