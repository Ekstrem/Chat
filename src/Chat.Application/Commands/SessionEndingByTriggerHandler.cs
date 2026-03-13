using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Commands
{
    public class SessionEndingByTriggerHandler : IRequestHandler<SessionEndingByTriggerCommand, ChatOperationResult>
    {
        private readonly IChatAggregateProvider _provider;

        public SessionEndingByTriggerHandler(IChatAggregateProvider provider)
        {
            _provider = provider;
        }

        public async Task<ChatOperationResult> Handle(SessionEndingByTriggerCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _provider.GetAggregateAsync(request.AggregateId, cancellationToken);

            var command = CommandToAggregate.Commit(
                Guid.NewGuid(),
                nameof(IChatAggregate.SessionEndingByTrigger),
                "SessionEndingByTrigger",
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            var model = AnemicModel.Create(
                request.AggregateId, command,
                null, null, null,
                Enumerable.Empty<IChatMessage>());

            var result = aggregate.SessionEndingByTrigger(model, command);

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
