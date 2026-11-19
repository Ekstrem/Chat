using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Implementation;

/// <summary>
/// Анемичная модель сообщения — контейнер состояния агрегата Message.
/// Неизменяема: любое изменение порождает новую модель.
/// </summary>
internal sealed class AnemicModel : IChatAnemicModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public long Version { get; init; }

    public string CommandName { get; init; } = string.Empty;

    public string SubjectName { get; init; } = string.Empty;

    public Guid CorrelationToken { get; init; }

    public IMessageRoot Root { get; init; } = MessageRoot.Empty();

    public IMessageText Text { get; init; } = MessageText.Empty();

    public IAddressee Addressee { get; init; } = MessageAddressee.Empty();

    public IAttachmentList Attachments { get; init; } = MessageAttachments.Empty();

    public Guid? ReplyTo { get; init; }

    public IExtendedFields ExtendedFields { get; init; } = MessageExtendedFields.Empty();

    public IReadOnlyList<IReadReceipt> ReadReceipts { get; init; } = Array.Empty<IReadReceipt>();

    public IDictionary<string, IValueObject> Invariants => GetValueObjects();

    public IDictionary<string, IValueObject> GetValueObjects()
        => new Dictionary<string, IValueObject>(StringComparer.Ordinal)
        {
            ["Root"] = Root,
            ["Text"] = Text,
            ["To"] = Addressee,
            ["Attachments"] = Attachments,
            ["Reply"] = new MessageReply(ReplyTo),
            ["ExtendedFields"] = ExtendedFields,
            ["ReadReceipts"] = new MessageReceipts(ReadReceipts)
        };

    /// <summary>Создать пустую модель с заданным идентификатором (fallback провайдера).</summary>
    public static AnemicModel Create(Guid id) => new() { Id = id };
}
