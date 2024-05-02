using MessagingApp.Data;
using MessagingApp.Dtos;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Services
{
    public class ChatService : IChatService
    {
        private readonly MessageAppDbContext _context;
        private readonly IUserService _userService;

        public ChatService(MessageAppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<ChatDto> GetChatInfo(string senderId, string receiverId)
        {
            bool swap = false;
            var chatDb = await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.ReceiverId == receiverId && x.SenderId == senderId);

            if (chatDb == null)
            {
                chatDb = await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.ReceiverId == senderId && x.SenderId == receiverId);
                swap = true;
            }

            if (chatDb == null)
            {
                chatDb = new Chat()
                {
                    Id = Guid.NewGuid().ToString(),
                    Messages = new List<Message>(),
                    ReceiverId = receiverId,
                    SenderId = senderId,
                    StartDate = DateTime.Now,
                };

                await _context.Chats.AddAsync(chatDb);
                await _context.SaveChangesAsync();
            }

            List<MessageDto> messages = new List<MessageDto>();

            foreach (var message in chatDb.Messages.OrderBy(x => x.Time))
            {
                messages.Add(new MessageDto()
                {
                    ChatId = message.ChatId,
                    Id = message.Id,
                    Text = message.Text,
                    UserId = message.UserId
                });
            }

            var receiver = await _userService.GetUserById(receiverId);
            var sender = await _userService.GetUserById(senderId);

            var chat = new ChatDto()
            {
                Id = chatDb.Id,
                ReceiverId = swap ? chatDb.SenderId : chatDb.ReceiverId,
                SenderId = swap ? chatDb.ReceiverId : chatDb.SenderId,
                StartDate = chatDb.StartDate,
                Receiver = new UserDto()
                {
                    Id = receiver.Id,
                    Image = receiver.ImageURL,
                    Username = receiver.UserName
                },
                Sender = new UserDto()
                {
                    Id = sender.Id,
                    Image = sender.ImageURL,
                    Username = sender.UserName
                },
                Messages = messages
            };

            return chat;
        }

        public async Task SendMessage(MessageDto message)
        {
            var messageDb = new Message()
            {
                ChatId = message.ChatId,
                Id = Guid.NewGuid().ToString(),
                Text = message.Text,
                UserId = message.UserId,
                Time = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(messageDb);
            await _context.SaveChangesAsync();
        }
    }
}
