using MediatR;
using System;

namespace Chat.Application.Commands
{
    /// <summary>
    /// Команда: абонент задал вопрос в чате.
    /// </summary>
    public class SubscriberRequestQuestionCommand : IRequest<ChatOperationResult>
    {
        public Guid UserId { get; init; }
        public int SessionId { get; init; }
        public string Login { get; init; }
        public string MessageText { get; init; }
        public string Platform { get; init; }
        public string Application { get; init; }
    }
}
