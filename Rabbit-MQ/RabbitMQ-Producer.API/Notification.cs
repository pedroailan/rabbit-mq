namespace RabbitMQ_Producer.API
{
    public class Notification
    {
        public string Message { get; set; } = new Guid().ToString();
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
