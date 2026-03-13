using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Commands
{
    public class OperatorDequeueRequestHandler : IRequestHandler<OperatorDequeueRequestCommand, ChatOperationResult>
    {
        private readonly IChatAggregateProvider _provider;

        public OperatorDequeueRequestHandler(IChatAggregateProvider provider)
        {
            _provider = provider;
        }

        public async Task<ChatOperationResult> Handle(OperatorDequeueRequestCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _provider.GetAggregateAsync(request.AggregateId, cancellationToken);

            var command = CommandToAggregate.Commit(
                Guid.NewGuid(),
                nameof(IChatAggregate.OperatorDequeueRequest),
                "OperatorDequeueRequest",
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            var model = AnemicModel.Create(
                request.AggregateId, command,
                null,
                ChatActor.CreateInstance(request.OperatorLogin, UserType.Operator),
                null,
                Enumerable.Empty<IChatMessage>());

            var result = aggregate.OperatorDequeueRequest(model, command);

            return new ChatOperationResult
            {
                AggregateId = request.AggregateId,
                Version = result.Event.Version,
                Result = result.Result.ToString(),
                Reason = result.Reason,
                IsSuccess = result.Result == DomainOperationResultEnum.Success
            };
        }
    }
}
