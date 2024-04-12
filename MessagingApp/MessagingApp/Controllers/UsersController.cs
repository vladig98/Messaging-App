using MessagingApp.Dtos;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("/login")]
        public async Task<ActionResult> Login(LoginDto loginData)
        {
            //prevents multiple sign ins
            if (User.Identity.IsAuthenticated)
            {
                await _userService.Logout();

                _logger.LogInformation("User logged out due to an attempt to log in again!");

                return Ok();
            }

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
