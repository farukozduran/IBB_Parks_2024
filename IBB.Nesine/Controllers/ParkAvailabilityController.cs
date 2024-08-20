using IBB.Nesine.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkAvailabilityController : Controller
    {
        private readonly IParkAvailabilityService _parkAvailabilityService;

        public ParkAvailabilityController(IParkAvailabilityService parkAvailabilityService)
        {
            _parkAvailabilityService = parkAvailabilityService;
        }

        [HttpGet("GetParkAvailability")]
        public ActionResult<bool> GetParkAvailability(int parkId)
        {
            return Ok(_parkAvailabilityService.GetParkAvailability(parkId));
        }
    }
}
