using Hive.SeedWorks.Events;
using System;
using System.Threading.Tasks;

namespace Chat.DomainServices
{
    public class EventBus :
        IEventBus
    {
        void IEventBusProducer.Publish<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent)
        {
            throw new NotImplementedException();
        }

        Task IEventBusProducer.PublishAsync<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent)
        {
            throw new NotImplementedException();
        }

        void IEventBusConsumer.Subscribe<TBoundedContext>(IDomainEventHandler<TBoundedContext> handler)
        {
            throw new NotImplementedException();
        }

        void IEventBusConsumer.Unsubscribe<TBoundedContext>(IDomainEventHandler<TBoundedContext> handler)
        {
            throw new NotImplementedException();
        }
    }
}
