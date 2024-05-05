using MessagingApp.Dtos;

namespace MessagingApp.Services.Contracts
{
    public interface IChatService
    {
        Task<ChatDto> GetChatInfo(string senderId, string receiverId);
        Task SendMessage(MessageDto message);
    }
}
