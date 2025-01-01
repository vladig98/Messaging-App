namespace MessagingApp.Dtos
{
    /// <summary>
    /// Represents a user DTO used for transferring user-related data.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the image URL of the user.
        /// </summary>
        public string Image { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the message associated with the user.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
