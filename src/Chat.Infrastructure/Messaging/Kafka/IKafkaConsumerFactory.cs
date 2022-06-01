using Confluent.Kafka;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public interface IKafkaConsumerFactory<TKey, TValue>
    {
        IConsumer<TKey, TValue> Create(ConsumerConfig config);
    }
}