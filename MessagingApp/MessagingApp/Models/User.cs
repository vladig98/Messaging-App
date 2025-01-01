using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace MessagingApp.Models
{
    public class User : IdentityUser<string>
    {
        public string RoleId { get; set; } = string.Empty;
        public Role Role { get; set; } = new();

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Chat> ReceiverChats { get; set; } = [];
        [JsonIgnore]
        public List<Chat> SenderChats { get; set; } = [];

        public string ImageURL { get; set; } = string.Empty;
    }
}
