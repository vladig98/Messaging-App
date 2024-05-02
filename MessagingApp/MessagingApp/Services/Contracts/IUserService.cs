using MessagingApp.Dtos;
using MessagingApp.Models;

namespace MessagingApp.Services.Contracts
{
    public interface IUserService
    {
        Task<string> Login(string username, string password);
        Task<string> Register(string username, string password, string firstName, string lastName, string email);
        Task<List<UserDto>> GetAllUsers(string email);
        Task<User> GetUserInfo(string id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(string id);
    }
}
