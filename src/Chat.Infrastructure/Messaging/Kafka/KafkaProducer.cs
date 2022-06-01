using Confluent.Kafka;
using Hive.SeedWorks.Definition;
using Hive.SeedWorks.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public class KafkaProducer : IExternalEventProducer
    {
        private readonly string _topic;
        private readonly ProducerConfig? _producerConfig;

        public KafkaProducer(IOptionsMonitor<KafkaConsumerOptions> options)
        {
            _topic = options.CurrentValue.Topics.FirstOrDefault()
                     ?? throw new ArgumentException("Kafka topic is empty.");

            _producerConfig = new ProducerConfig(options.CurrentValue.Config);
        }

        public async Task Publish<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent) where TBoundedContext : IBoundedContext
        {
            using var p = new ProducerBuilder<string, string>(_producerConfig).Build();
            await p.ProduceAsync(
                _topic,
                new Message<string, string>
                {
                    Key = domainEvent
                            .GetType()
                            .Name,
                    Value = JsonConvert.SerializeObject(domainEvent)
                });
        }
    }
}
