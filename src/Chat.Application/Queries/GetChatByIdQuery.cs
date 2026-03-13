using Chat.Domain.Abstraction;
using MediatR;
using System;
using System.Collections.Generic;

namespace Chat.Application.Queries
{
    /// <summary>
    /// Запрос: получить чат по идентификатору.
    /// </summary>
    public class GetChatByIdQuery : IRequest<List<IChatAnemicModel>>
    {
        public Guid AggregateId { get; init; }
    }
}
