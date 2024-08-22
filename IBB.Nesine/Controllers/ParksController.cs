using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using IBB.Nesine.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParksController : Controller
    {
        private readonly IParkService _parkService;

        public ParksController(IParkService parkService)
        {
            _parkService = parkService;
        }

        [HttpGet("GetParkByDistrict")]
        public ActionResult<List<GetParksByDistrictResponseModel>> GetParkByDistrict(string district)
        {
            return Ok(_parkService.GetParksByDistrict(district));
        }

        [HttpGet("GetParkAvailabilityByParkId")]
        public ActionResult<bool> GetParkAvailabilityByParkId(int parkId)
        {
            return Ok(_parkService.GetParkAvailabilityByParkId(parkId));
        }
    }
}
