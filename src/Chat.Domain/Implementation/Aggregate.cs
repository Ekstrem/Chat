using Chat.Domain.Abstraction;
using Chat.Domain.Specifications;
using DigiTFactory.Libraries.SeedWorks.Characteristics;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Implementation;

/// <summary>
/// РђРіСЂРµРіР°С‚ Message: РєРѕРјР°РЅРґС‹ SendMessage / AddAttachment / ReadMessage.
/// РљР°Р¶РґР°СЏ РєРѕРјР°РЅРґР° РІРѕР·РІСЂР°С‰Р°РµС‚ <see cref="AggregateResult{TBoundedContext,TModel}"/> СЃ РґРѕРјРµРЅРЅС‹Рј СЃРѕР±С‹С‚РёРµРј.
/// </summary>
internal sealed class Aggregate : IChatAggregate
{
    private readonly IChatAnemicModel _model;

    private Aggregate(IChatAnemicModel model) => _model = model;

    public static Aggregate CreateInstance(IChatAnemicModel model) => new(model);

    public Guid Id => _model.Id;

    public long Version => _model.Version;

    public string CommandName => _model.CommandName;

    public string SubjectName => _model.SubjectName;

    public Guid CorrelationToken => _model.CorrelationToken;

    public IDictionary<string, IValueObject> Invariants => _model.Invariants;

    public IDictionary<string, IValueObject> GetValueObjects() => _model.GetValueObjects();

    /// <summary>РЎРѕР·РґР°С‚СЊ СЃРѕРѕР±С‰РµРЅРёРµ (РєРѕРјР°РЅРґР° SendMessage).</summary>
    public AggregateResult<IChat, IChatAnemicModel> SendMessage(
        ICommandToAggregate command,
        string product,
        string role,
        string to,
        string text,
        IEnumerable<Guid>? attachments,
        Guid? replyTo,
        IReadOnlyDictionary<string, string>? extendedFields)
    {
        if (!MessageText.TryCreate(text, out var messageText, out var textReason))
        {
            return Fail(command, textReason);
        }

        if (!MessageAddressee.TryCreate(to, out var addressee, out var toReason))
        {
            return Fail(command, toReason);
        }

        var version = ResolveVersion(command);
        var candidate = new AnemicModel
        {
            Id = _model.Id == Guid.Empty ? Guid.NewGuid() : _model.Id,
            Version = version,
            CommandName = command.CommandName,
            SubjectName = command.SubjectName,
            CorrelationToken = command.CorrelationToken,
            Root = MessageRoot.CreateInstance(product, role, version),
            Text = messageText,
            Addressee = addressee,
            Attachments = MessageAttachments.CreateInstance(attachments),
            ReplyTo = replyTo,
            ExtendedFields = MessageExtendedFields.CreateInstance(extendedFields),
            ReadReceipts = Array.Empty<IReadReceipt>()
        };

        var data = BusinessOperationData<IChat, IChatAnemicModel>.Commit<IChat, IChatAnemicModel>(_model, candidate);
        var textValidator = new MessageTextValidator();
        var addresseeValidator = new AddresseeValidator();
        var newMessageValidator = new NewMessageValidator();

        return textValidator.IsSatisfiedBy(data) && addresseeValidator.IsSatisfiedBy(data) && newMessageValidator.IsSatisfiedBy(data)
            ? new AggregateResultSuccess<IChat, IChatAnemicModel>(data)
            : new AggregateResultException<IChat, IChatAnemicModel>(data, newMessageValidator, textValidator, addresseeValidator);
    }

    /// <summary>Р”РѕР±Р°РІРёС‚СЊ РІР»РѕР¶РµРЅРёРµ (РєРѕРјР°РЅРґР° AddAttachment).</summary>
    public AggregateResult<IChat, IChatAnemicModel> AddAttachment(ICommandToAggregate command, Guid attachmentId)
    {
        var data = BusinessOperationData<IChat, IChatAnemicModel>.Commit<IChat, IChatAnemicModel>(_model, _model);
        var existing = new ExistingMessageValidator();
        if (!existing.IsSatisfiedBy(data))
        {
            return Fail(command, existing.Reason);
        }

        if (attachmentId == Guid.Empty)
        {
            return Fail(command, "attachmentId: РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РІР»РѕР¶РµРЅРёСЏ РѕР±СЏР·Р°С‚РµР»РµРЅ");
        }

        var attachments = _model.Attachments.Items.Contains(attachmentId)
            ? _model.Attachments.Items
            : _model.Attachments.Items.Append(attachmentId).ToList();

        var candidate = new AnemicModel
        {
            Id = _model.Id,
            Version = ResolveVersion(command),
            CommandName = command.CommandName,
            SubjectName = command.SubjectName,
            CorrelationToken = command.CorrelationToken,
            Root = _model.Root,
            Text = _model.Text,
            Addressee = _model.Addressee,
            Attachments = MessageAttachments.CreateInstance(attachments),
            ReplyTo = _model.ReplyTo,
            ExtendedFields = _model.ExtendedFields,
            ReadReceipts = _model.ReadReceipts
        };

        return new AggregateResultSuccess<IChat, IChatAnemicModel>(
            BusinessOperationData<IChat, IChatAnemicModel>.Commit<IChat, IChatAnemicModel>(_model, candidate));
    }

    /// <summary>РћС‚РјРµС‚РёС‚СЊ СЃРѕРѕР±С‰РµРЅРёРµ РїСЂРѕС‡РёС‚Р°РЅРЅС‹Рј (РєРѕРјР°РЅРґР° ReadMessage).</summary>
    public AggregateResult<IChat, IChatAnemicModel> ReadMessage(ICommandToAggregate command, string addressee)
    {
        var data = BusinessOperationData<IChat, IChatAnemicModel>.Commit<IChat, IChatAnemicModel>(_model, _model);
        var existing = new ExistingMessageValidator();
        if (!existing.IsSatisfiedBy(data))
        {
            return Fail(command, existing.Reason);
        }

        if (string.IsNullOrWhiteSpace(addressee))
        {
            return Fail(command, "addressee: Р°РґСЂРµСЃР°С‚ РѕР±СЏР·Р°С‚РµР»РµРЅ");
        }

        var moment = ResolveVersion(command);
        var receipts = _model.ReadReceipts
            .Where(receipt => !string.Equals(receipt.Addressee, addressee, StringComparison.Ordinal))
            .Append(MessageReadReceipt.CreateInstance(addressee, moment))
            .ToList();

        var candidate = new AnemicModel
        {
            Id = _model.Id,
            Version = moment,
            CommandName = command.CommandName,
            SubjectName = command.SubjectName,
            CorrelationToken = command.CorrelationToken,
            Root = _model.Root,
            Text = _model.Text,
            Addressee = _model.Addressee,
            Attachments = _model.Attachments,
            ReplyTo = _model.ReplyTo,
            ExtendedFields = _model.ExtendedFields,
            ReadReceipts = receipts
        };

        return new AggregateResultSuccess<IChat, IChatAnemicModel>(
            BusinessOperationData<IChat, IChatAnemicModel>.Commit<IChat, IChatAnemicModel>(_model, candidate));
    }

    private static long ResolveVersion(ICommandToAggregate command)
        => command.Version > 0
            ? command.Version
            : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private AggregateResult<IChat, IChatAnemicModel> Fail(ICommandToAggregate command, string reason)
    {
        _ = command;
        return new AggregateResultException<IChat, IChatAnemicModel>(
            BusinessOperationData<IChat, IChatAnemicModel>.Commit<IChat, IChatAnemicModel>(_model, _model),
            new FailedOperationSpecification(reason));
    }
}
