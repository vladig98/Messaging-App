using MessagingApp.Dtos;
using MessagingApp.Enums;
using MessagingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessagingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<UsersController> logger,
            IConfiguration configuration, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        private string GenerateToken(string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              new List<Claim>()
              {
                  new Claim(JwtRegisteredClaimNames.Sub, userId)
              },
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(Sectoken);
        }

        private JwtSecurityToken DecryptToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            //tokenS.Claims.First(x => x.Type == "sub").Value

            return tokenS;
        }

        [HttpPost("/login")]
        public async Task<ActionResult> Login(LoginDto loginData)
        {
            var result = await _signInManager.PasswordSignInAsync(loginData.Username, loginData.Password, false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            _logger.LogInformation("User logged in.");

            var user = await _userManager.FindByNameAsync(loginData.Username);

            var token = GenerateToken(user.Id);

            DecryptToken(token);

            return Ok(token);
        }

        [HttpPost("/register")]
        public async Task<ActionResult> Register(RegisterDto registerData)
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

            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = registerData.Email,
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                UserName = registerData.Username,
                RoleId = role.Id,
                Role = role
            };

            var result = await _userManager.CreateAsync(user, registerData.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
                await _userManager.AddClaimAsync(user, claim: new Claim(ClaimTypes.Role.ToString(), role.Name));
                _logger.LogInformation("User created a new account with password.");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError(error.Description);
            }

            var token = GenerateToken(user.Id);

            return Ok(token);
        }
    }
}
