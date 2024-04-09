using MessagingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagingController : Controller
    {
        private readonly ILogger<MessagingController> _logger;

        public MessagingController(ILogger<MessagingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public Message Get(string id)
        {
            var message = new Message()
            {
                ChatId = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                Text = "This is a test message",
                UserId = Guid.NewGuid().ToString()
            };

            return message;
        }

        [HttpPost("send")]
        public string PostMessage(string message)
        {
            return message;
        }
    }
}
