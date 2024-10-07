using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly TokenHelper _tokenHelper;

        public UserController(IUserService userService, TokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        [HttpPost("Register")]
        public async Task<string> AddUser(UserModel user)
        {
            if (ModelState.IsValid)
            {
                return await _userService.Register(user);
            }

            return "Model is not valid!";
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserModel user)
        {
            if (_userService.CheckLoginInfo(user))
            {
                var token = _tokenHelper.GetJwtToken(user);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }
    }
}
