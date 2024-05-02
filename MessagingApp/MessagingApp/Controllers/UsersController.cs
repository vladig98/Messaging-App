using MessagingApp.Dtos;
using MessagingApp.Filters;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace MessagingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("/getusers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;

            var users = await _userService.GetAllUsers(email);

            return Ok(users);
        }

        [HttpPost("/login")]
        [CheckLoggedInFilter]
        public async Task<ActionResult> Login(LoginDto loginData)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid data provided for login!");
                return BadRequest("Incorrect values provided!");
            }

            string token = await _userService.Login(loginData.Username, loginData.Password);

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Failed to retireved token!");

                return BadRequest("Invalid data entered!");
            }

            _logger.LogInformation("User logged in successfully!");

            return Ok(token);
        }

        [HttpPost("/register")]
        [CheckLoggedInFilter]
        public async Task<ActionResult> Register(RegisterDto registerData)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid data provided for registration!");
                return BadRequest("Incorrect values provided!");
            }

            string token = await _userService.Register(registerData.Username, registerData.Password, registerData.FirstName, registerData.LastName, registerData.Email);

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Failed to retireved token!");
                return BadRequest("Invalid data entered!");
            }

            _logger.LogInformation("User registered successfully!");

            return Ok(token);
        }
    }
}
