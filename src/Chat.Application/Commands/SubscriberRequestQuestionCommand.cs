using System;
using MediatR;

namespace Chat.Application.Commands
{
    public class SubscriberRequestQuestionCommand : IRequest<ChatOperationResult>
    {
        public Guid AggregateId { get; set; }
        public Guid CorrelationToken { get; set; }
        public string SubscriberName { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}
