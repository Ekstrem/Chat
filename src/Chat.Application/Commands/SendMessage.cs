using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using MediatR;

namespace Chat.Application.Commands;

/// <summary>РљРѕРјР°РЅРґР° SendMessage: СЃРѕР·РґР°С‚СЊ СЃРѕРѕР±С‰РµРЅРёРµ РІ РєР°РЅР°Р»Рµ.</summary>
public sealed record SendMessageCommand(
    Guid CorrelationToken,
    string Product,
    string Role,
    string To,
    string Text,
    Guid[]? Attachments = null,
    Guid? ReplyTo = null,
    Dictionary<string, string>? ExtendedFields = null) : IRequest<ChatOperationResult>;

/// <summary>РћР±СЂР°Р±РѕС‚С‡РёРє SendMessage: Р°РіСЂРµРіР°С‚ в†’ РёРЅРІР°СЂРёР°РЅС‚С‹ в†’ СЃРѕР±С‹С‚РёРµ С‡РµСЂРµР· С€РёРЅСѓ.</summary>
public sealed class SendMessageHandler : ICommandHandler, IRequestHandler<SendMessageCommand, ChatOperationResult>
{
    private readonly IAggregateProvider<IChat, IChatAggregate> _provider;

    public SendMessageHandler(IAggregateProvider<IChat, IChatAggregate> provider) => _provider = provider;

    public async Task<ChatOperationResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var command = CommandToAggregate.Commit(request.CorrelationToken, "SendMessage", request.Role);
        var aggregate = await _provider.GetAggregateAsync(Guid.NewGuid(), cancellationToken).ConfigureAwait(false);

        var result = aggregate.SendMessage(
            command,
            request.Product,
            request.Role,
            request.To,
            request.Text,
            request.Attachments,
            request.ReplyTo,
            request.ExtendedFields);

        return result.Result == DomainOperationResultEnum.Success
            ? ChatOperationResult.Ok(result.Event.Id)
            : ChatOperationResult.Fail(result.Reason);
    }
}
