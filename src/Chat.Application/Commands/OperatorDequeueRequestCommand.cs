using System;
using MediatR;

namespace Chat.Application.Commands
{
    public class OperatorDequeueRequestCommand : IRequest<ChatOperationResult>
    {
        public Guid AggregateId { get; set; }
        public Guid CorrelationToken { get; set; }
        public string OperatorName { get; set; } = string.Empty;
    }
}
