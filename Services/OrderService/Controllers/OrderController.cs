using Microsoft.AspNetCore.Mvc;
using OrderService.EventBus;
using OrderService.EventBus.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRabbitMqProducer _rabbitMqProducer;

        public OrderController(IRabbitMqProducer rabbitMqProducer)
        {
            _rabbitMqProducer = rabbitMqProducer; 
        }

        [HttpPost("create")]
        public IActionResult CreateOrder([FromBody]OrderMessage order)
        {
           _rabbitMqProducer.SendMessage(order);
            return Ok("Order created and message sent to RabbitMQ");
        }
    }
}
