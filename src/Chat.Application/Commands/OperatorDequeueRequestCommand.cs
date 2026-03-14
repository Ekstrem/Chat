using MediatR;
using System;

namespace Chat.Application.Commands
{
    /// <summary>
    /// Команда: оператор взял обращение на обработку.
    /// </summary>
    public class OperatorDequeueRequestCommand : IRequest<ChatOperationResult>
    {
        public Guid AggregateId { get; set; }
        public string OperatorLogin { get; set; }
    }
}
