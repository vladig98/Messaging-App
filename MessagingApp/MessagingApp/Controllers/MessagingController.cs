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
        public string Get(string id)
        {
            return "You successfuly pulled a message!";
        }

        [HttpPost("send")]
        public string PostMessage(string message)
        {
            return message;
        }
    }
}
