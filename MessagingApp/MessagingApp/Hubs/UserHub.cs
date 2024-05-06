using MessagingApp.Dtos;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Hubs
{
    public class UserHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserHub> _logger;

        public UserHub(IUserService userService, ILogger<UserHub> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //gets user's information
        [Authorize]
        public async Task GetUserInfo(UserInfoDto data)
        {
            //gets the user that is chatting with me
            var user = await _userService.GetUserInfo(data.Id);

            _logger.LogInformation($"User {user.UserName} retrieved successfully!");

            //generates the JSON response
            var userDto = user != null ? new UserDto()
            {
                Id = user.Id,
                Image = user.ImageURL,
                Username = user.UserName
            } : null;

            _logger.LogInformation("Sending user data to the signalR client.");
            //sends back to the client
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserInfo", userDto);
        }
    }
}
