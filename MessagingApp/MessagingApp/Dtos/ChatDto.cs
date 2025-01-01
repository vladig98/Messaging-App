namespace MessagingApp.Dtos
{
    /// <summary>
    /// Represents a chat DTO used for transferring chat-related data.
    /// </summary>
    public class ChatDto
    {
        /// <summary>
        /// Gets or sets the ID of the chat.
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the start date of the chat.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Gets or sets the ID of the sender user.
        /// </summary>
        public string SenderId { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the sender user DTO.
        /// </summary>
        public UserDto Sender { get; set; } = new();
        // <summary>
        /// Gets or sets the ID of the receiver user.
        /// </summary>
        public string ReceiverId { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the receiver user DTO.
        /// </summary>
        public UserDto Receiver { get; set; } = new();
        /// <summary>
        /// Gets or sets the list of messages in the chat.
        /// </summary>
        public List<MessageDto> Messages { get; set; } = [];
    }
}
