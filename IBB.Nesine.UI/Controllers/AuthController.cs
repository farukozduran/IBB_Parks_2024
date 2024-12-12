using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace IBB.Nesine.UI.Controllers
{
	[Route("api/auth")]
	public class AuthController : Controller
	{
		private readonly TokenHelper _tokenHelper;
		private IUserService _userService;

		public AuthController(IUserService userService, TokenHelper tokenHelper)
		{
			_userService = userService;
			_tokenHelper = tokenHelper;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserModel user)
		{
			if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
			{
				return BadRequest("Invalid credentials.");
			}

			if (!_userService.CheckLoginInfo(user))
			{
				return Unauthorized("Invalid username or password.");
			}

			// JWT Token oluştur
			var token = _tokenHelper.GetJwtToken(user);
			return Ok(new { token });
		}

		[HttpPost("register")]
		public async Task<string> Register([FromBody] UserModel user)
		{
			if (ModelState.IsValid)
			{
				return await _userService.Register(user);
			}
			return "Model is not valid!";
		}
		[HttpGet("login")]
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet("register")]
		public IActionResult Register()
		{
			return View();
		}
	}
}
