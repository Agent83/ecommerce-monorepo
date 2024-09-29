using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container
builder.Services.AddControllers();

//Configure JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"))
        };
    });

var app = builder.Build();

app.MapGet("/send", () =>
{
    var factory = new ConnectionFactory() { HostName = "rabbitmq" };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    channel.QueueDeclare(queue: "user_queue",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    string message = "User registered!";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "",
                         routingKey: "user_queue",
                         basicProperties: null,
                         body: body);

    return "Message Sent to RabbitMQ!";
});




// use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();