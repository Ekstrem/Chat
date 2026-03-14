using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.DomainServices;
using DigiTFactory.Libraries.SeedWorks.Characteristics;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using MediatR;

namespace Chat.Application.Commands
{
    public class OperatorRepliedToMessageHandler : IRequestHandler<OperatorRepliedToMessageCommand, ChatOperationResult>
    {
        private readonly IChatAggregateProvider _aggregateProvider;

        public OperatorRepliedToMessageHandler(IChatAggregateProvider aggregateProvider)
        {
            _aggregateProvider = aggregateProvider;
        }

        public async Task<ChatOperationResult> Handle(OperatorRepliedToMessageCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _aggregateProvider.GetAggregateAsync(request.AggregateId, cancellationToken);

            var commandMetadata = CommandToAggregate.Commit(
                request.CorrelationToken,
                nameof(OperatorRepliedToMessageCommand),
                "Operator");

            var result = aggregate.OperatorRepliedToMessage(aggregate, commandMetadata);

            if (result.Result == DomainOperationResultEnum.Success)
            {
                return ChatOperationResult.Success(
                    request.AggregateId,
                    aggregate.Version + 1,
                    result.Result.ToString());
            }

            return ChatOperationResult.Failure(result.Reason.ToArray());
        }
    }
}
