using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Commands
{
    public class OperatorRepliedToMessageHandler : IRequestHandler<OperatorRepliedToMessageCommand, ChatOperationResult>
    {
        private readonly IChatAggregateProvider _provider;

        public OperatorRepliedToMessageHandler(IChatAggregateProvider provider)
        {
            _provider = provider;
        }

        public async Task<ChatOperationResult> Handle(OperatorRepliedToMessageCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _provider.GetAggregateAsync(request.AggregateId, cancellationToken);

            var command = CommandToAggregate.Commit(
                Guid.NewGuid(),
                nameof(IChatAggregate.OperatorRepliedToMessage),
                "OperatorRepliedToMessage",
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            var model = AnemicModel.Create(
                request.AggregateId, command,
                null, null, null,
                new[]
                {
                    ChatMessage.CreateInstance(
                        MessageType.Text, request.MessageText,
                        Platform.Windows, Chat.Domain.Abstraction.Application.Site, Guid.Empty)
                });

            var result = aggregate.OperatorRepliedToMessage(model, command);

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
