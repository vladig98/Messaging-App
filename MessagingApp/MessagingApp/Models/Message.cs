namespace MessagingApp.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }

        public Chat Chat { get; set; }
        public string ChatId { get; set; }
    }
}
