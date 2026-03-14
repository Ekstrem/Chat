using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigiTFactory.Libraries.SeedWorks.Definition;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Chat.DomainServices
{
    /// <summary>
    /// Реализация IEventBus через MassTransit + RabbitMQ.
    /// </summary>
    public class RabbitMqEventBus : IEventBus
    {
        private readonly IBus _bus;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();

        public RabbitMqEventBus(IBus bus, ILogger<RabbitMqEventBus> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public void Publish<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent)
            where TBoundedContext : IBoundedContext
        {
            PublishAsync(domainEvent).GetAwaiter().GetResult();
        }

        public async Task PublishAsync<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent)
            where TBoundedContext : IBoundedContext
        {
            var envelope = ToEnvelope(domainEvent);

            _logger.LogInformation(
                "Публикация доменного события: {CommandName}, Version: {Version}, CorrelationToken: {CorrelationToken}",
                envelope.CommandName, envelope.Version, envelope.CorrelationToken);

            await _bus.Publish(envelope);

            _logger.LogDebug("Доменное событие опубликовано в RabbitMQ.");
        }

        public void Subscribe<TBoundedContext>(IDomainEventHandler<TBoundedContext> handler)
            where TBoundedContext : IBoundedContext
        {
            var key = typeof(TBoundedContext);
            var list = _handlers.GetOrAdd(key, _ => new List<object>());
            lock (list)
            {
                list.Add(handler);
            }

            _logger.LogInformation(
                "Подписка на доменные события контекста {BoundedContext}.", key.Name);
        }

        public void Unsubscribe<TBoundedContext>(IDomainEventHandler<TBoundedContext> handler)
            where TBoundedContext : IBoundedContext
        {
            var key = typeof(TBoundedContext);
            if (_handlers.TryGetValue(key, out var list))
            {
                lock (list)
                {
                    list.Remove(handler);
                }

                _logger.LogInformation(
                    "Отписка от доменных событий контекста {BoundedContext}.", key.Name);
            }
        }

        private static DomainEventEnvelope ToEnvelope<TBoundedContext>(IDomainEvent<TBoundedContext> domainEvent)
            where TBoundedContext : IBoundedContext
        {
            return new DomainEventEnvelope
            {
                AggregateId = domainEvent.Id,
                Version = domainEvent.Version,
                CorrelationToken = domainEvent.Command?.CorrelationToken ?? Guid.Empty,
                BoundedContext = typeof(TBoundedContext).Name,
                CommandName = domainEvent.Command?.CommandName,
                SubjectName = domainEvent.Command?.SubjectName,
                ChangedValueObjectsJson = JsonConvert.SerializeObject(domainEvent.ChangedValueObjects),
                Result = domainEvent.Result.ToString(),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
