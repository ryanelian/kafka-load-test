using CapMemoryTest.Messages;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace CapMemoryTest.Controllers
{
    public class MyMessageSubscriber : Controller
    {
        private readonly ILogger<MyMessageSubscriber> _logger;

        public MyMessageSubscriber(ILogger<MyMessageSubscriber> logger)
        {
            _logger = logger;
        }

        [NonAction]
        [CapSubscribe("my.message")]
        public void ReceiveMessage(MyMessage model)
        {
            _logger.LogInformation("Received new message: {MessageID}", model.MessageID);
        }
    }
}
