using Chat.Infrastructure.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Api.BackgroundServices.EventBusConsumer
{
    public class EventBusMessageHandler : IKafkaMessageHandler<string, string>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventBusMessageHandler(IServiceScopeFactory serviceScopeFactory) 
            => _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        public async Task Handle(Message<string, string> message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var readModelConsumer = scope.ServiceProvider.GetRequiredService<IEventBusConsumer>();
            var messageFactory = GetMessageFactory(scope);

            var request = messageFactory.CreateMessage(message);
            await readModelConsumer.Consume(request, cancellationToken);
        }

        private static IMessageFactory<string, string> GetMessageFactory(IServiceScope scope) 
            => scope.ServiceProvider.GetRequiredService<EventMessageFactory>();
    }
}
