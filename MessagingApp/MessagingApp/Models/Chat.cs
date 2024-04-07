namespace MessagingApp.Models
{
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
        }

        public string Id { get; set; }
        public DateTime StartDate { get; set; }

        public string SenderId { get; set; }
        public User Sender { get; set; }

        public string ReceiverId { get; set; }
        public User Receiver { get; set; }

        public List<Message> Messages { get; set; }
    }
}
