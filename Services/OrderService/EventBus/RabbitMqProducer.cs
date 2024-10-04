using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.EventBus
{
    public class RabbitMqProducer : IRabbitMqProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqProducer()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void SendMessage<T>(T message)
        {
           var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(exchange: "",
                                  routingKey: "order_queue",
                                  basicProperties: null,
                                  body: body);
        }
    }
}