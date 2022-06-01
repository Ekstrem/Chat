using Confluent.Kafka;
using Chat.InternalContracts.Messaging;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public interface IMessageFactory<TKey, TValue>
    {
        IMessage CreateMessage(Message<TKey, TValue> message);
    }
}