using Microsoft.AspNetCore.Identity;

namespace MessagingApp.Models
{
    public class Role : IdentityRole<string>
    {
        public Role()
        {
            Users = new List<User>();
        }

        public List<User> Users { get; set; }
    }
}
