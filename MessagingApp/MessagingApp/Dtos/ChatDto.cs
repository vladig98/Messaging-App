namespace MessagingApp.Dtos
{
    public class ChatDto
    {
        public ChatDto()
        {
            Messages = new List<MessageDto>();
        }

        public string Id { get; set; }
        public DateTime StartDate { get; set; }

        public string SenderId { get; set; }
        public UserDto Sender { get; set; }

        public string ReceiverId { get; set; }
        public UserDto Receiver { get; set; }

        public List<MessageDto> Messages { get; set; }
    }
}
