using System;
using MediatR;

namespace Chat.Application.Queries
{
    public class GetChatByIdQuery : IRequest<ChatOperationResult>
    {
        public Guid AggregateId { get; set; }
    }
}
