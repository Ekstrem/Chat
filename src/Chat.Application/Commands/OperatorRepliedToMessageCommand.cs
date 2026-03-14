using MediatR;
using System;

namespace Chat.Application.Commands
{
    /// <summary>
    /// Команда: оператор ответил на обращение.
    /// </summary>
    public class OperatorRepliedToMessageCommand : IRequest<ChatOperationResult>
    {
        public Guid AggregateId { get; set; }
        public string MessageText { get; set; }
    }
}
