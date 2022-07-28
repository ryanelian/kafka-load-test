using CapMemoryTest.Messages;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapMemoryTest.Controllers
{

    [Route("api/cap")]
    [ApiController]
    public class CapMessageApiController : ControllerBase
    {
        private readonly ICapPublisher _capPublisher;

        public CapMessageApiController(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> Get(CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("my.message", new MyMessage
            {
                MessageID = Guid.NewGuid()
            }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
