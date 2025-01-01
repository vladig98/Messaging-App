using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Dtos
{
    /// <summary>
    /// Represents a DTO for user login.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Length(5, 32)]
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Length(8, 32)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
