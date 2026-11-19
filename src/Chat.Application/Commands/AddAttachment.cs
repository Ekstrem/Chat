using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using MediatR;

namespace Chat.Application.Commands;

/// <summary>Команда AddAttachment: добавить вложение к существующему сообщению.</summary>
public sealed record AddAttachmentCommand(
    Guid CorrelationToken,
    Guid MessageId,
    Guid AttachmentId,
    string Role) : IRequest<ChatOperationResult>;

/// <summary>Обработчик AddAttachment.</summary>
public sealed class AddAttachmentHandler : ICommandHandler, IRequestHandler<AddAttachmentCommand, ChatOperationResult>
{
    private readonly IAggregateProvider<IChat, IChatAggregate> _provider;

    public AddAttachmentHandler(IAggregateProvider<IChat, IChatAggregate> provider) => _provider = provider;

    public async Task<ChatOperationResult> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
    {
        var command = CommandToAggregate.Commit(request.CorrelationToken, "AddAttachment", request.Role);
        var aggregate = await _provider.GetAggregateAsync(request.MessageId, cancellationToken).ConfigureAwait(false);

        var result = aggregate.AddAttachment(command, request.AttachmentId);

        return result.Result == DomainOperationResultEnum.Success
            ? ChatOperationResult.Ok(request.MessageId)
            : ChatOperationResult.Fail(result.Reason);
    }
}
