namespace MessagingApp.Dtos
{
    /// <summary>
    /// Represents a chat DTO used for transferring chat-related data.
    /// </summary>
    public class ChatDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDto"/> class.
        /// </summary>
        public ChatDto()
        {
            Messages = new List<MessageDto>();
        }

        /// <summary>
        /// Gets or sets the ID of the chat.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the start date of the chat.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Gets or sets the ID of the sender user.
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// Gets or sets the sender user DTO.
        /// </summary>
        public UserDto Sender { get; set; }
        // <summary>
        /// Gets or sets the ID of the receiver user.
        /// </summary>
        public string ReceiverId { get; set; }
        /// <summary>
        /// Gets or sets the receiver user DTO.
        /// </summary>
        public UserDto Receiver { get; set; }
        /// <summary>
        /// Gets or sets the list of messages in the chat.
        /// </summary>
        public List<MessageDto> Messages { get; set; }
    }
}
