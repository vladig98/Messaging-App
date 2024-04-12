using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Dtos
{
    public class RegisterDto
    {
        [Length(3, 20)]
        public string FirstName { get; set; }

        [Length(3, 20)]
        public string LastName { get; set; }

        [Length(8, 32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress]
        [Length(5, 50)]
        public string Email { get; set; }

        [Length(5, 32)]
        public string Username { get; set; }
    }
}
