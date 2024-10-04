using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace UserService.EventsBus
{
    public class MessagePublisher: IMessagePublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessagePublisher()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task PublishAsync<T>(T message, string exchange, string routingKey)
        {
            return Task.Run(() =>
            {
                _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

                var jsonMessage = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _channel.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

            });
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
