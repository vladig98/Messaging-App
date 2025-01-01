using MessagingApp.Data;
using MessagingApp.Dtos;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Services
{
    public class ChatService(MessageAppDbContext context, IUserService userService, ILogger<ChatService> logger) : IChatService
    {
        private readonly MessageAppDbContext _context = context;
        private readonly IUserService _userService = userService;
        private readonly ILogger<ChatService> _logger = logger;

        public async Task<ChatDto> GetChatInfo(string senderId, string receiverId)
        {
            (bool isSenderTheSender, Chat chat) = await GetOrCreate(senderId, receiverId);

            List<MessageDto> messages = [];

            foreach (Message message in chat.Messages.OrderBy(x => x.Time))
            {
                messages.Add(new()
                {
                    ChatId = message.ChatId,
                    Id = message.Id,
                    Text = message.Text,
                    UserId = message.UserId
                });
            }

            User receiver = await _userService.GetUserById(receiverId);
            User sender = await _userService.GetUserById(senderId);

            ChatDto chatDto = new()
            {
                Id = chat.Id,
                ReceiverId = isSenderTheSender ? chat.SenderId : chat.ReceiverId,
                SenderId = isSenderTheSender ? chat.ReceiverId : chat.SenderId,
                StartDate = chat.StartDate,
                Receiver = new()
                {
                    Id = receiver.Id,
                    Image = receiver.ImageURL,
                    Username = receiver.UserName ?? string.Empty
                },
                Sender = new()
                {
                    Id = sender.Id,
                    Image = sender.ImageURL,
                    Username = sender.UserName ?? string.Empty
                },
                Messages = messages
            };

            return chatDto;
        }

        public async Task SendMessage(MessageDto message)
        {
            Message messageDb = new()
            {
                ChatId = message.ChatId,
                Text = message.Text,
                UserId = message.UserId
            };

            await _context.Messages.AddAsync(messageDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Message Id: {Id} for ChatId: {ChatId} sent from UserId: {UserId} saved successfully!", messageDb.Id, messageDb.ChatId, messageDb.UserId);
        }

        private async Task<(bool, Chat)> GetOrCreate(string senderId, string receiverId)
        {
            Chat? chat = await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.ReceiverId == receiverId && x.SenderId == senderId);
            chat ??= await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.ReceiverId == senderId && x.SenderId == receiverId);

            if (chat == null)
            {
                chat = new()
                {
                    ReceiverId = receiverId,
                    SenderId = senderId
                };

                await _context.Chats.AddAsync(chat);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chat between SenderId: {SenderId} and ReceiverId: {ReceiverId} created successfully!", senderId, receiverId);
            }

            bool isSenderTheSender = chat.SenderId == senderId;

            return (isSenderTheSender, chat);
        }
    }
}
