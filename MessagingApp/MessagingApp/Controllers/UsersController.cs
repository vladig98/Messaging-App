using MessagingApp.Dtos;
using MessagingApp.Enums;
using MessagingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessagingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("/login")]
        public string Login()
        {
            return "JWT";
        }

        [HttpPost("/register")]
        public async Task<string> Register(UserDto user)
        {
            var role = new Role()
            {
                Name = RoleName.User.ToString(),
                Id = Guid.NewGuid().ToString()
            };

            bool doesRoleExist = await _roleManager.RoleExistsAsync(role.Name);

            if (!doesRoleExist)
            {
                await _roleManager.CreateAsync(role);
            }

            var appUser = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Username,
                RoleId = role.Id,
                Role = role
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);
            await _userManager.AddToRoleAsync(appUser, role.Name);
            await _userManager.AddClaimAsync(appUser, claim: new Claim(ClaimTypes.Role.ToString(), role.Name));

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError(error.Description);
            }

            return "register";
        }
    }
}
