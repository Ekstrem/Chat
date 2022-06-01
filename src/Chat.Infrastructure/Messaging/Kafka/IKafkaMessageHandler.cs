using Confluent.Kafka;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public interface IKafkaMessageHandler<TKey, TValue>
    {
        Task Handle(Message<TKey, TValue> message, CancellationToken cancellationToken);
    }
}