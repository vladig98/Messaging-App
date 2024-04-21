using MessagingApp.Data;
using MessagingApp.Dtos;
using MessagingApp.Enums;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using MessagingApp.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        private List<string> ProfilePictures {  get; set; }

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

        public async Task<List<UserDto>> GetAllUsers()
        {
            //var currentUser = await _userManager.FindByNameAsync(username);

            //var messages = await _messageService.GetAllMessages(currentUser);

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

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

            if (!result.Succeeded)
            {
                _logger.LogError($"Invalid credentials for user {username}");
                return null;
            }

            _logger.LogInformation($"User {username} logged in.");

            var token = HelperMethods.GenerateToken(user.Id, _configuration);

            _logger.LogInformation($"Token generated for user {username}");

            return token;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User signed out!");
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
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError(error.Description);
            }

            var token = HelperMethods.GenerateToken(user.Id, _configuration);

            _logger.LogInformation($"Token for user {username} generated!");

            return token;
        }
    }
}
