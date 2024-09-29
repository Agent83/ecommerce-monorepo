using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// RabbitMQ Consumer setup
app.MapGet("/receive", () =>
{
    var factory = new ConnectionFactory() { HostName = "localhost" }; // use "rabbitmq" for Docker
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    channel.QueueDeclare(queue: "user_queue",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);
    };

    channel.BasicConsume(queue: "user_queue",
                         autoAck: true,
                         consumer: consumer);

    return "Listening for messages...";
});

app.Run();
