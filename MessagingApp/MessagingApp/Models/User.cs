using Microsoft.AspNetCore.Identity;

namespace MessagingApp.Models
{
    public class User : IdentityUser<string>
    {
        public User()
        {
            ReceiverChats = new List<Chat>();
            SenderChats = new List<Chat>();
        }

        public string Id { get; set; }

        public string RoleId { get; set; }
        public Role Role { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public List<Chat> ReceiverChats { get; set; }
        public List<Chat> SenderChats { get; set; }
    }
}
