using Chat.Contracts.Views;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Queries
{
    internal class QueriesHandler :
        IRequestHandler<GetChatInfoQuery, ChatInfoView>
    {
        public Task<ChatInfoView> Handle(GetChatInfoQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
