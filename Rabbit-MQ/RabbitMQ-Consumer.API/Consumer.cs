using Commons;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ_Consumer.API
{
    public class Consumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Consumer(IConfiguration configuration)
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

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            _channel.BasicConsume(queue: "api_queue",
                                 autoAck: true,
                                 consumer: consumer);
        }

        ~Consumer()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}