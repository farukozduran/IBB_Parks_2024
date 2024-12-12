using IBB.Nesine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.UI.Controllers
{
    public class ParksController : Controller
    {
        private readonly IParkService _parkService;

        public ParksController(IParkService parkService)
        {
            _parkService = parkService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetParksList()
        {
            var parks = _parkService.GetParkList();
            return View(parks);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateParksInfo()
        {
            var success = _parkService.UpdateParksInfoAsync();
            return Json(new { success });
        }

        [HttpGet]
        public IActionResult GetParksByDistrict(string district)
        {
            var parks = _parkService.GetParksByDistrict(district);
            return Json(parks);
        }
        //[Authorize]
        [HttpGet]
        public IActionResult GetParkAvailabilityByParkId(int parkId)
        {
            var isAvailable = _parkService.GetParkAvailabilityByParkId(parkId);
            if(isAvailable == null)
            {
                return NotFound(new { message = "Park bulunamadi!" });
            }
            return Ok(new {isAvailable });
        }
    }
}
