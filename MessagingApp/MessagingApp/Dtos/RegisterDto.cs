using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Dtos
{
    /// <summary>
    /// Represents a DTO for registering a new user.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [Length(3, 20)]
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [Length(3, 20)]
        public string LastName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        [Length(8, 32)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [EmailAddress]
        [Length(5, 50)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Length(5, 32)]
        public string Username { get; set; } = string.Empty;
    }
}
