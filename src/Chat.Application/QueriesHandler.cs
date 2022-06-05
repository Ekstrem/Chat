using AutoMapper;
using Chat.Contracts.Views;
using Chat.InternalContracts;
using Chat.MaterializedView.Dialogs.Specifications;
using MediatR;
using Hive.SeedWorks.Monads;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Chat.Application.Queries
{
    internal class QueriesHandler :
        IRequestHandler<GetChatInfoQuery, ChatInfoView>
    {
        private readonly IQueryRepository<DialogView> _repository;
        private readonly IMapper _mapper;

        public QueriesHandler(
            IQueryRepository<DialogView> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ChatInfoView> Handle(GetChatInfoQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetByIdSpec(request.DialogId);
            return (await _repository.FindByAsync(spec, cancellationToken))
                .PipeTo(r => _mapper.Map<ChatInfoView>(r));
        }
    }
}
