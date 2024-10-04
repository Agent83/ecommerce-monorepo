using OrderService.EventBus.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderService.EventBus
{
    public class RabbitMqConsumer : IRabbitMqConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare( queue: "order_queue",
                                   durable: false,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null );
        }
        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderMessage = JsonSerializer.Deserialize<OrderMessage>(message);
                Console.WriteLine($"Order received: ID = {orderMessage.OrderId}, Status = {orderMessage.OrderStatus}, Customer = {orderMessage.CustomerId}");
            };
            _channel.BasicConsume(queue: "order_queue", 
                                  autoAck: true,
                                  consumer: consumer);
        }
    }
}
