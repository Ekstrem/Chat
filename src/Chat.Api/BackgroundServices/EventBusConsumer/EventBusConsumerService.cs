using Chat.Infrastructure.Messaging.Kafka;
using Microsoft.Extensions.Options;

namespace Chat.Api.BackgroundServices.EventBusConsumer
{
    public class EventBusConsumerService : KafkaConsumerBackgroundService<string, string>
    {
        public EventBusConsumerService(IOptionsMonitor<KafkaConsumerOptions> optionsMonitor,
            IKafkaConsumerFactory<string, string> consumerFactory, EventBusMessageHandler handler)
            : base(optionsMonitor, consumerFactory, handler)
        {
        }
    }
}
