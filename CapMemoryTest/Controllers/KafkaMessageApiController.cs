using CapMemoryTest.Messages;
using Confluent.Kafka;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapMemoryTest.Controllers
{

    [Route("api/kafka")]
    [ApiController]
    public class KafkaMessageApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KafkaMessageApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> Get(CancellationToken cancellationToken)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _configuration.GetConnectionString("Kafka")
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var data = new MyMessage
            {
                MessageID = Guid.NewGuid()
            };
            var message = new Message<Null, string>
            {
                Value = System.Text.Json.JsonSerializer.Serialize(data)
            };
            var result = await producer.ProduceAsync("confluent.kafka-memory-test", message, cancellationToken);

            return result.Status == PersistenceStatus.Persisted;
        }
    }
}
