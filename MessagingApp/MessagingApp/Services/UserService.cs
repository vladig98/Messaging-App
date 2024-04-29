using MessagingApp.Data;
using MessagingApp.Dtos;
using MessagingApp.Enums;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessagingApp.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly MessageAppDbContext _messageAppDbContext;
        private List<string> ProfilePictures { get; set; }

        public UserService(ILogger<IUserService> logger, UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IConfiguration configuration, MessageAppDbContext messageAppDbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _messageAppDbContext = messageAppDbContext;
            ProfilePictures = new List<string>()
            {
                @"https://static-00.iconduck.com/assets.00/user-avatar-1-icon-2048x2048-935gruik.png",
                @"https://static-00.iconduck.com/assets.00/user-avatar-devil-icon-256x255-1clwb7tp.png",
                @"https://static-00.iconduck.com/assets.00/user-avatar-glad-icon-256x255-cbmmpmut.png",
                @"https://upload.wikimedia.org/wikipedia/commons/1/1e/User_%281%29.png",
                @"https://cdn4.iconfinder.com/data/icons/education-circular-1-1/96/40-512.png"
            };
        }

        private async Task<string> GenerateJWTToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            }.Union(userClaims);

            var token = new JwtSecurityToken(
                    issuer: _configuration["MessagingApp:JWT:Issuer"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(3600),
                    audience: _configuration["MessagingApp:JWT:Issuer"],
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["MessagingApp:JWT:Key"])), SecurityAlgorithms.HmacSha256)
                );

            _logger.LogInformation($"Token generated for user {user.UserName}");

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            List<UserDto> users = new List<UserDto>();

            var dbUsers = await _messageAppDbContext.Users.ToListAsync();

            foreach (var user in dbUsers)
            {
                var message = await _messageAppDbContext.Messages.FirstOrDefaultAsync(m => m.UserId == user.Id);

                users.Add(new UserDto
                {
                    Username = user.UserName,
                    Message = message == null ? string.Empty : (string.IsNullOrEmpty(message.Text) ? string.Empty : message.Text),
                    Image = user.ImageURL
                });
            }

            return users;
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                _logger.LogError($"User {username} does not exist!");
                return null;
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                _logger.LogError($"Invalid credentials for user {username}");
                return null;
            }

            return await GenerateJWTToken(user);
        }

        public async Task<string> Register(string username, string password, string firstName, string lastName, string email)
        {
            var userDB = await _userManager.FindByNameAsync(username);

            if (userDB != null)
            {
                _logger.LogError($"User {username} already exists!");
                return null;
            }

            userDB = await _userManager.FindByEmailAsync(email);

            if (userDB != null)
            {
                _logger.LogError($"User with email {email} already exists!");
                return null;
            }

            var role = new Role()
            {
                Name = RoleName.User.ToString(),
                Id = Guid.NewGuid().ToString()
            };

            bool doesRoleExist = await _roleManager.RoleExistsAsync(role.Name);

            if (!doesRoleExist)
            {
                await _roleManager.CreateAsync(role);
                _logger.LogInformation("Role created successfully!");
            }

            Random rnd = new Random();

            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserName = username,
                RoleId = role.Id,
                Role = role,
                ImageURL = ProfilePictures.ElementAt(rnd.Next(ProfilePictures.Count))
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
                await _userManager.AddClaimAsync(user, claim: new Claim(ClaimTypes.Role.ToString(), role.Name));
                _logger.LogInformation($"User {username} created a new account with password.");
                _logger.LogInformation($"User {username} assigned to role {role.Name}.");

                return await GenerateJWTToken(user);
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError(error.Description);
            }

            return null;
        }
    }
}
