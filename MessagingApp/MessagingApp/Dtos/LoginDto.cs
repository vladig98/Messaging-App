using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Dtos
{
    public class LoginDto
    {
        [Length(5, 32)]
        public string Username { get; set; }

        [Length(8, 32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
