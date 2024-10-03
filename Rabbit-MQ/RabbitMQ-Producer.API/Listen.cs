using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Commons;
using System.Text;
using System.Text.Json;

namespace RabbitMQ_Producer.API
{
    public class Listen
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<Listen> _logger;

        public Listen(IConfiguration configuration, ILogger<Listen> logger)
        {
            ConnectionFactory factory = Builder.Build(configuration);
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "api_queue_response",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _logger = logger;
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
                long latency = message.CalculateTime(receivedTimestamp);


                _logger.LogInformation("{0};{1}", DateTime.Now.ToString("HH:mm:ss:fff"), latency);
            };
            _channel.BasicConsume(queue: "api_queue_response",
                                 autoAck: true,
                                 consumer: consumer);
        }

        ~Listen()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
