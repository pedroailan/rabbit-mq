using Commons;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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
                var str = Encoding.UTF8.GetString(body);

                Notification message = JsonSerializer.Deserialize<Notification>(str);

                // Tempo atual (recebimento da mensagem)
                long receivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Calcular a diferença entre o envio e o recebimento
                long latency = receivedTimestamp - message.Timestamp;

                Console.WriteLine(" [x] Received {0}", message.Message);
                Console.WriteLine($"Tempo total entre produção e consumo: {latency} ms");
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