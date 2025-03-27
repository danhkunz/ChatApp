namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
    }
}