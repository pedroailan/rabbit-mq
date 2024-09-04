using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ_Producer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(Producer producer) : ControllerBase
    {
        private readonly Producer _producer = producer;

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] string message)
        {
            _producer.Notification(message);
            return Ok("Message sent to RabbitMQ");
        }
    }
}
