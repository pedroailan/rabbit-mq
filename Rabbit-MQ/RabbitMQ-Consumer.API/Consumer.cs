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
        private readonly Response _response;
        private readonly ILogger<Consumer> _logger;

        public Consumer(IConfiguration configuration, Response response, ILogger<Consumer> logger)
        {
            ConnectionFactory factory = Builder.Build(configuration);
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "api_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _response = response;
            _logger = logger;
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received +=  (model, ea)  =>
            {
                var body = ea.Body.ToArray();
                var str = Encoding.UTF8.GetString(body);

                Notification message = JsonSerializer.Deserialize<Notification>(str);

                // Tempo atual (recebimento da mensagem)
                long receivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Calcular a diferença entre o envio e o recebimento
                long latency = message.CalculateTime(receivedTimestamp);

                _response.Notification(message);
                _logger.LogInformation("{0};{1}", DateTime.Now.ToString("HH:mm:ss:fff"), latency);
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