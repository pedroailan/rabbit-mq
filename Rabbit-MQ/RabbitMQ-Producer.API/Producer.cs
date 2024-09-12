using Commons;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQ_Producer.API
{
    public class Producer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Producer(IConfiguration configuration)
        {
            ConnectionFactory factory = Builder.Build(configuration);
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "api_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void Notification(Notification notification)
        {
            var message = new
            {
                Content = notification.Message,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var messageBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "api_queue",
                                 basicProperties: null,
                                 body: body);
        }

        ~Producer()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}