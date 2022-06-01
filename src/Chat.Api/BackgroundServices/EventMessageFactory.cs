using Chat.Domain;
using Chat.Infrastructure.Messaging.Kafka;
using Chat.InternalContracts;
using Chat.InternalContracts.Messaging;
using Confluent.Kafka;
using Hive.SeedWorks.Definition;
using Hive.SeedWorks.Events;
using Newtonsoft.Json;
using System;

namespace Chat.Api.BackgroundServices
{
    public class EventMessageFactory : IMessageFactory<string, string>
    {
        public IMessage CreateMessage(Message<string, string> message)
        {
            var e = GetEvent<IChat>(message);
            return e switch
            {
                DomainEvent<IChat> => new ChatDomainEventCommand(e),

                _ => throw new NotSupportedException($"Event '{e.GetType()}' is not supported."),
            };
        }

        private IDomainEvent<TBoundedContext> GetEvent<TBoundedContext>(Message<string, string> message)
            where TBoundedContext : IBoundedContext
            => JsonConvert.DeserializeObject<DomainEvent<TBoundedContext>>(message.Value);
    }
}
