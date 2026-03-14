using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using DigiTFactory.Libraries.SeedWorks.Result;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Commands
{
    public class SubscriberRequestQuestionHandler : IRequestHandler<SubscriberRequestQuestionCommand, ChatOperationResult>
    {
        private readonly IChatAggregateProvider _provider;

        public SubscriberRequestQuestionHandler(IChatAggregateProvider provider)
        {
            _provider = provider;
        }

        public async Task<ChatOperationResult> Handle(SubscriberRequestQuestionCommand request, CancellationToken cancellationToken)
        {
            var aggregateId = Guid.NewGuid();
            var aggregate = await _provider.GetAggregateAsync(aggregateId, cancellationToken);

            var platform = Enum.Parse<Platform>(request.Platform ?? "Windows", ignoreCase: true);
            var application = Enum.Parse<Chat.Domain.Abstraction.Application>(request.Application ?? "Site", ignoreCase: true);

            var model = AnemicModel.Create(
                aggregateId,
                default, Guid.NewGuid(),
                nameof(IChatAggregate.SubscriberRequestQuestion),
                "SubscriberRequestQuestion",
                ChatRoot.CreateInstance(request.UserId, request.SessionId),
                ChatActor.CreateInstance(request.Login, UserType.Client),
                null,
                new[]
                {
                    ChatMessage.CreateInstance(
                        MessageType.Text, request.MessageText,
                        platform, application, Guid.Empty)
                });

            var result = aggregate.SubscriberRequestQuestion(model);

            return new ChatOperationResult
            {
                AggregateId = aggregateId,
                Version = result.Event.Version,
                Result = result.Result.ToString(),
                Reason = result.Reason,
                IsSuccess = result.Result == DomainOperationResultEnum.Success
            };
        }
    }
}
