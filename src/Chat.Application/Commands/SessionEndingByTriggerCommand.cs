using MediatR;
using System;

namespace Chat.Application.Commands
{
    /// <summary>
    /// Команда: сессия завершилась по триггеру.
    /// </summary>
    public class SessionEndingByTriggerCommand : IRequest<ChatOperationResult>
    {
        public Guid AggregateId { get; init; }
    }
}
