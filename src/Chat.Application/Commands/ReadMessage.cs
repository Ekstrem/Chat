using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using MediatR;

namespace Chat.Application.Commands;

/// <summary>Команда ReadMessage: отметить сообщение прочитанным адресатом.</summary>
public sealed record ReadMessageCommand(
    Guid CorrelationToken,
    Guid MessageId,
    string Addressee) : IRequest<ChatOperationResult>;

/// <summary>Обработчик ReadMessage.</summary>
public sealed class ReadMessageHandler : ICommandHandler, IRequestHandler<ReadMessageCommand, ChatOperationResult>
{
    private readonly IAggregateProvider<IChat, IChatAggregate> _provider;

    public ReadMessageHandler(IAggregateProvider<IChat, IChatAggregate> provider) => _provider = provider;

    public async Task<ChatOperationResult> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        var command = CommandToAggregate.Commit(request.CorrelationToken, "ReadMessage", request.Addressee);
        var aggregate = await _provider.GetAggregateAsync(request.MessageId, cancellationToken).ConfigureAwait(false);

        var result = aggregate.ReadMessage(command, request.Addressee);

        return result.Result == DomainOperationResultEnum.Success
            ? ChatOperationResult.Ok(request.MessageId)
            : ChatOperationResult.Fail(result.Reason);
    }
}
