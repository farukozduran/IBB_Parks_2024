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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("AddUser")]
        public async Task<string> AddUser(UserModel user)
        {
            if (ModelState.IsValid)
            {
                return await _userService.AddUser(user);
            }

            return "Model is not valid!";
        }
    }
}
