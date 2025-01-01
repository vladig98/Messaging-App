namespace MessagingApp.Dtos
{
    /// <summary>
    /// Represents a message DTO used for transferring message-related data.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// Gets or sets the ID of the message.
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the ID of the user who sent the message.
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the ID of the chat to which the message belongs.
        /// </summary>
        public string ChatId { get; set; } = string.Empty;
    }
}
