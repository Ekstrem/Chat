using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Chat.DomainServices.Consumers
{
    public class DomainEventConsumer : IConsumer<DomainEventEnvelope>
    {
        private readonly ILogger<DomainEventConsumer> _logger;

        public DomainEventConsumer(ILogger<DomainEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<DomainEventEnvelope> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received domain event: AggregateId={AggregateId}, Version={Version}, Command={CommandName}, BoundedContext={BoundedContext}",
                message.AggregateId,
                message.Version,
                message.CommandName,
                message.BoundedContext);

            // TODO: Обработка полученного события (обновление read model, уведомления и т.д.)

            return Task.CompletedTask;
        }
    }
}
