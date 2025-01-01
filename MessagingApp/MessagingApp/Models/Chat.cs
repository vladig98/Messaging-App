namespace MessagingApp.Models
{
    public class Chat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public string SenderId { get; set; } = string.Empty;
        public User Sender { get; set; } = new();

        public string ReceiverId { get; set; } = string.Empty;
        public User Receiver { get; set; } = new();

        public List<Message> Messages { get; set; } = [];
    }
}
