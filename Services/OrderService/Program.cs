using OrderService.EventBus;

var builder = WebApplication.CreateBuilder(args);

// Add RabbitMQ producer and consumer
builder.Services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();
builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

var app = builder.Build();

var rabbitMqConsumer = app.Services.GetRequiredService<IRabbitMqConsumer>();
rabbitMqConsumer.StartListening();

app.UseAuthorization();

app.MapControllers();

app.Run();

