using MessagingApp.Dtos;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Hubs
{
    public class UserHub(IUserService userService, ILogger<UserHub> logger) : Hub
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<UserHub> _logger = logger;

        [Authorize]
        public async Task GetUserInfo(UserInfoDto data)
        {
            User user = await _userService.GetUserInfo(data.Id);

            _logger.LogInformation("User {UserName} retrieved successfully!", user.UserName);

            UserDto? userDto = user == null ? null : new()
            {
                Id = user.Id,
                Image = user.ImageURL,
                Username = user.UserName ?? string.Empty
            };

            _logger.LogInformation("Sending user data to the signalR client.");

            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserInfo", userDto);
        }
    }
}
