using Commons;
using RabbitMQ.Client;
using System.Text;

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
            var body = Encoding.UTF8.GetBytes(notification.Message);

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