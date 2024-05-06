using MessagingApp.Dtos;
using MessagingApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MessagingApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, IUserService userService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _userService = userService;
            _logger = logger;
        }

        //retrieves the email form the JWT token / user claims
        private string ExtractEmailFromJWT(HubCallerContext context)
        {
            var email = context.User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;

            _logger.LogInformation($"Email {email} extracted successfully from the claims");

            return email;
        }

        //gets a user using their email
        private async Task<string> ExtractUserIdWithEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);

            _logger.LogInformation($"User with email {email} found.");

            return user.Id;
        }

        //get the chat info
        [Authorize]
        public async Task GetChatInfo(UserInfoDto data)
        {
            var email = ExtractEmailFromJWT(Context);
            string userId = await ExtractUserIdWithEmail(email);

            var chat = await _chatService.GetChatInfo(userId, data.Id);
            _logger.LogInformation($"Chat data extracted successfully.");

            //sends the data only to the one who requested it
            _logger.LogInformation($"Sending chat data to signalR client");
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveChatInfo", chat);
        }

        //sends a message to the db
        [Authorize]
        public async Task SendMessage(MessageDto message)
        {
            var email = ExtractEmailFromJWT(Context);
            string userId = await ExtractUserIdWithEmail(email);

            //sets the user based on the JWT
            message.UserId = userId;

            await _chatService.SendMessage(message);
            _logger.LogInformation($"Message {message} stored successfully");

            //sending to all clients
            _logger.LogInformation($"Sending MessageSentSuccessfully to the signalR client");
            await Clients.All.SendAsync("MessageReceived", "MessageSentSuccessfully");
        }
    }
}
