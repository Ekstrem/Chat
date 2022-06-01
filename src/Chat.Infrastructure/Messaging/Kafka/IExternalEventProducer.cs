using Hive.SeedWorks.Events;
using Hive.SeedWorks.Definition;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public interface IExternalEventProducer
    {
        Task Publish<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent) where TBoundedContext : IBoundedContext;
    }
}
