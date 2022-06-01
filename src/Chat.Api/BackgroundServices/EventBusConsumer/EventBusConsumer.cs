using Chat.InternalContracts.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Api.BackgroundServices.EventBusConsumer
{
    public class EventBusConsumer: IEventBusConsumer
    {
        private readonly IMediator _mediator;

        public EventBusConsumer(IMediator mediator) 
            => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task Consume<TMessage>(TMessage message, CancellationToken cancellationToken)
            where TMessage : IMessage 
            => await _mediator.Publish(message, cancellationToken);
    }
}
