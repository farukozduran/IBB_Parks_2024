using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserModel user)
        {
            if (_authService.CheckLoginInfo(user))
            {
                var token =  _authService.GetJwtToken(user);
                return Ok(new {Token = token});
            }
            return Unauthorized();
        }
    }
}
