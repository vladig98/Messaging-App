using MessagingApp.Dtos;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Hubs
{
    public class UserHub : Hub
    {
        private readonly IUserService _userService;

        public UserHub(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        public async Task GetUserInfo(UserInfoDto data)
        {
            var user = await _userService.GetUserInfo(data.Id);

            var userDto = user == null ? null : new UserDto()
            {
                Id = user.Id,
                Image = user.ImageURL,
                Username = user.UserName
            };

            await Clients.All.SendAsync("ReceiveUserInfo", userDto);
        }
    }
}
