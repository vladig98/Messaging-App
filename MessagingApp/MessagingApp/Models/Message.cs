namespace MessagingApp.Models
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = string.Empty;
        public User User { get; set; } = new();
        public string UserId { get; set; } = string.Empty;

        public Chat Chat { get; set; } = new();
        public string ChatId { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
