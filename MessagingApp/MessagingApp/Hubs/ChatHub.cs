using MessagingApp.Dtos;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MessagingApp.Hubs
{
    public class ChatHub(IChatService chatService, IUserService userService, ILogger<ChatHub> logger) : Hub
    {
        private readonly IChatService _chatService = chatService;
        private readonly IUserService _userService = userService;
        private readonly ILogger<ChatHub> _logger = logger;

        [Authorize]
        public async Task GetChatInfo(UserInfoDto data)
        {
            string email = ExtractEmailFromJWT(Context);
            string userId = await ExtractUserIdWithEmail(email);

            ChatDto chat = await _chatService.GetChatInfo(userId, data.Id);

            _logger.LogInformation("Chat data extracted successfully.");
            _logger.LogInformation("Sending chat data to signalR client");

            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveChatInfo", chat);
        }

        [Authorize]
        public async Task SendMessage(MessageDto message)
        {
            string email = ExtractEmailFromJWT(Context);
            string userId = await ExtractUserIdWithEmail(email);

            message.UserId = userId;

            await _chatService.SendMessage(message);

            _logger.LogInformation("Message {Message} stored successfully", message);
            _logger.LogInformation("Sending MessageSentSuccessfully to the signalR client");

            await Clients.All.SendAsync("MessageReceived", "MessageSentSuccessfully");
        }

        private string ExtractEmailFromJWT(HubCallerContext context)
        {
            string email = context.User?.Claims.First(claim => claim.Type == ClaimTypes.Email).Value ?? string.Empty;

            _logger.LogInformation("Email {Email} extracted successfully from the claims", email);

            return email;
        }

        private async Task<string> ExtractUserIdWithEmail(string email)
        {
            User user = await _userService.GetUserByEmail(email);

            _logger.LogInformation("User with email {Email} found.", email);

            return user.Id;
        }
    }
}
