using Microsoft.AspNetCore.Identity;

namespace MessagingApp.Models
{
    public class Role : IdentityRole<string>
    {
        public List<User> Users { get; set; } = [];
    }
}
