using CapMemoryTest.Messages;
using Confluent.Kafka;
using System.Text.Json;

namespace CapMemoryTest.HostedServices
{
    public class SubscribeKafkaBackgroundService : BackgroundService
    {
        private readonly ILogger<SubscribeKafkaBackgroundService> _logger;
        private readonly ConsumerBuilder<Ignore, string> _consumerBuilder;

        public SubscribeKafkaBackgroundService(IConfiguration configuration, ILogger<SubscribeKafkaBackgroundService> logger)
        {
            _logger = logger;
            var broker = configuration.GetConnectionString("Kafka");
            var config = new ConsumerConfig
            {
                GroupId = "group1",
                BootstrapServers = broker,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true
            };
            _consumerBuilder = new ConsumerBuilder<Ignore, string>(config);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();
            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            using var consumer = _consumerBuilder.Build();
            consumer.Subscribe("confluent.kafka-memory-test");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);
                    var myMessage = JsonSerializer.Deserialize<MyMessage>(result.Message.Value);
                    _logger.LogInformation("Received message ID: {MessageID}", myMessage?.MessageID);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    if (ex.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        _logger.LogCritical(ex, "Fatal Kafka consume error: {Reason}", ex.Error.Reason);
                        break;
                    } else
                    {
                        // Consumer errors should generally be ignored (or logged) unless fatal.
                        _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error");
                    break;
                }
            }
        }
    }
}
