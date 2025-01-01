using MessagingApp.Dtos;
using MessagingApp.Filters;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessagingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(ILogger<UsersController> logger, IUserService userService) : Controller
    {
        private readonly ILogger<UsersController> _logger = logger;
        private readonly IUserService _userService = userService;

        [HttpGet("/getusers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            string email = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            List<UserDto> users = await _userService.GetAllUsers(email);

            _logger.LogInformation("User with email {Email} retrieved all users", email);

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

            if (!IsTokenValid(token))
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

            if (!IsTokenValid(token))
            {
                _logger.LogError("Failed to retireved token!");

                return BadRequest("Invalid data entered!");
            }

            _logger.LogInformation("User registered successfully!");

            return Ok(token);
        }

        private static bool IsTokenValid(string token)
        {
            return !string.IsNullOrEmpty(token) && !string.IsNullOrWhiteSpace(token);
        }
    }
}
