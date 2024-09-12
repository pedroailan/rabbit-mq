namespace RabbitMQ_Producer.API
{
    public class Job(Producer producer) : BackgroundService
    {
        private readonly Producer _producer = producer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Starting...");
                _producer.Notification(new Notification());
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}