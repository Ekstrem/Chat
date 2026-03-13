using Chat.Domain.Abstraction;
using Chat.DomainServices;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Queries
{
    public class GetChatByIdHandler : IRequestHandler<GetChatByIdQuery, List<IChatAnemicModel>>
    {
        private readonly IRepository _repository;

        public GetChatByIdHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<IChatAnemicModel>> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetById(request.AggregateId, cancellationToken);
        }
    }
}
