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
    public class UserService(ILogger<IUserService> logger, UserManager<User> userManager,
        RoleManager<Role> roleManager, IConfiguration configuration, MessageAppDbContext messageAppDbContext) : IUserService
    {
        private readonly ILogger<IUserService> _logger = logger;
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Role> _roleManager = roleManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly MessageAppDbContext _messageAppDbContext = messageAppDbContext;
        private List<string> ProfilePictures { get; set; } =
            [
                @"https://static-00.iconduck.com/assets.00/user-avatar-1-icon-2048x2048-935gruik.png",
                @"https://static-00.iconduck.com/assets.00/user-avatar-devil-icon-256x255-1clwb7tp.png",
                @"https://static-00.iconduck.com/assets.00/user-avatar-glad-icon-256x255-cbmmpmut.png",
                @"https://upload.wikimedia.org/wikipedia/commons/1/1e/User_%281%29.png",
                @"https://cdn4.iconfinder.com/data/icons/education-circular-1-1/96/40-512.png"
            ];

        public async Task<List<UserDto>> GetAllUsers(string email)
        {
            List<UserDto> users = [];
            List<User> dbUsers = await _messageAppDbContext.Users.ToListAsync();

            foreach (User user in dbUsers)
            {
                if (user.Email == email)
                {
                    continue;
                }

                Message? lastMessage = await _messageAppDbContext.Messages.Where(m => m.UserId == user.Id).OrderBy(x => x.Time).LastOrDefaultAsync();

                users.Add(new()
                {
                    Username = user.UserName ?? string.Empty,
                    Message = lastMessage?.Text ?? string.Empty,
                    Image = user.ImageURL,
                    Id = user.Id
                });
            }

            return users;
        }

        public async Task<string> Login(string username, string password)
        {
            User? user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                _logger.LogError("User {Username} does not exist!", username);

                return string.Empty;
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

            if (!isPasswordValid)
            {
                _logger.LogError("Invalid credentials for user {Username}", username);

                return string.Empty;
            }

            return await GenerateJWTToken(user);
        }

        public async Task<string> Register(string username, string password, string firstName, string lastName, string email)
        {
            User? userDB = await _userManager.FindByNameAsync(username);

            if (userDB != null)
            {
                _logger.LogError("User {Username} already exists!", username);

                return string.Empty;
            }

            userDB = await _userManager.FindByEmailAsync(email);

            if (userDB != null)
            {
                _logger.LogError("User with email {Email} already exists!", email);

                return string.Empty;
            }

            Role role = new()
            {
                Name = RoleName.User.ToString()
            };

            bool doesRoleExist = await _roleManager.RoleExistsAsync(role.Name);

            if (!doesRoleExist)
            {
                await _roleManager.CreateAsync(role);

                _logger.LogInformation("Role created successfully!");
            }

            Random rnd = new();

            User user = new()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserName = username,
                RoleId = role.Id,
                Role = role,
                ImageURL = ProfilePictures.ElementAt(rnd.Next(ProfilePictures.Count))
            };

            IdentityResult result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    _logger.LogError("{Error}", error.Description);
                }

                return string.Empty;
            }

            await _userManager.AddToRoleAsync(user, role.Name);
            await _userManager.AddClaimAsync(user, claim: new Claim(ClaimTypes.Role.ToString(), role.Name));

            _logger.LogInformation("User {Username} created a new account with password.", username);
            _logger.LogInformation("User {Username} assigned to role {Role}.", username, role.Name);

            return await GenerateJWTToken(user);
        }

        public async Task<User> GetUserInfo(string id)
        {
            User? user = await _userManager.FindByIdAsync(id);

            return user ?? throw new InvalidOperationException($"User not found Id: {id}!");
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email) ?? throw new InvalidOperationException($"User not found Email: {email}!");
        }

        public async Task<User> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id) ?? throw new InvalidOperationException($"User not found Id: {id}!");
        }

        private async Task<string> GenerateJWTToken(User user)
        {
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            IEnumerable<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            ];

            claims = claims.Union(userClaims);

            JwtSecurityToken token = new(
                issuer: _configuration["MessagingApp:JWT:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(3600),
                audience: _configuration["MessagingApp:JWT:Issuer"],
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["MessagingApp:JWT:Key"] ?? throw new InvalidOperationException("No JWT key!"))), 
                    SecurityAlgorithms.HmacSha256)
            );

            _logger.LogInformation("Token generated for user {UserName}", user.UserName);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
