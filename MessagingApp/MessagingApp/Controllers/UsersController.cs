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
            //gets the email from the claims
            var email = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            
            //gets all users excluding the current user
            var users = await _userService.GetAllUsers(email);

            _logger.LogInformation($"User with email ${email} retrieved all users");

            return Ok(users);
        }

        //check if the JWT token is not empty or null
        private bool IsTokenValid(string token)
        {
            return string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token);
        }

        [HttpPost("/login")]
        [CheckLoggedInFilter]
        public async Task<ActionResult> Login(LoginDto loginData)
        {
            //data validation
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid data provided for login!");
                return BadRequest("Incorrect values provided!");
            }

            //generates a token
            //imitates user login
            string token = await _userService.Login(loginData.Username, loginData.Password);

            //if the token creatin failed due to bad credentials, return a 400
            if (IsTokenValid(token))
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
            //data validation
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid data provided for registration!");
                return BadRequest("Incorrect values provided!");
            }

            //creates a token
            //simulates a log in upon successful registration
            string token = await _userService.Register(registerData.Username, registerData.Password, registerData.FirstName, registerData.LastName, registerData.Email);

            //if the token creatin failed due to bad credentials, return a 400
            if (IsTokenValid(token))
            {
                _logger.LogError("Failed to retireved token!");
                return BadRequest("Invalid data entered!");
            }

            _logger.LogInformation("User registered successfully!");

            return Ok(token);
        }
    }
}
