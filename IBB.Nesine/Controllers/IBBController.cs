using IBB.Nesine.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IBBController : Controller
    {
        private readonly IIBBService _ibbService;

        public IBBController(IIBBService ibbService)
        {
            _ibbService = ibbService;
        }

        [HttpPost("UpdateIBBParksInfo")]
        public async Task<bool> UpdateIBBParksInfoAsync()
        {
            return await _ibbService.UpdateIBBParksInfoAsync();
        }
    }
}
