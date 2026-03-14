using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.DomainServices;
using DigiTFactory.Libraries.SeedWorks.Result;
using MediatR;

namespace Chat.Application.Commands
{
    public class SubscriberRequestQuestionHandler : IRequestHandler<SubscriberRequestQuestionCommand, ChatOperationResult>
    {
        private readonly IChatAggregateProvider _aggregateProvider;

        public SubscriberRequestQuestionHandler(IChatAggregateProvider aggregateProvider)
        {
            _aggregateProvider = aggregateProvider;
        }

        public async Task<ChatOperationResult> Handle(SubscriberRequestQuestionCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _aggregateProvider.GetAggregateAsync(request.AggregateId, cancellationToken);

            var result = aggregate.SubscriberRequestQuestion(aggregate);

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
