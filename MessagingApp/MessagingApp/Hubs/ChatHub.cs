using MessagingApp.Dtos;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace MessagingApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatHub(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        private string ExtractEmailFromJWT(HubCallerContext context)
        {
            var accessToken = context.GetHttpContext().Request.Query["access_token"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken);
            var tokenS = jsonToken as JwtSecurityToken;

            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;

            return email;
        }

        private async Task<string> ExtractUserIdWithEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);

            return user.Id;
        }

        [Authorize]
        public async Task GetChatInfo(UserInfoDto data)
        {
            var email = ExtractEmailFromJWT(Context);
            string userId = await ExtractUserIdWithEmail(email);

            var chat = await _chatService.GetChatInfo(userId, data.Id);

            //await Clients.Client(Context.ConnectionId).SendAsync("ReceiveChatInfo", chat);
            await Clients.All.SendAsync("ReceiveChatInfo", chat);
        }

        [Authorize]
        public async Task SendMessage(MessageDto message)
        {
            var email = ExtractEmailFromJWT(Context);
            string userId = await ExtractUserIdWithEmail(email);

            message.UserId = userId;

            await _chatService.SendMessage(message);

            await Clients.All.SendAsync("MessageReceived", "MessageSentSuccessfully");
        }
    }
}
