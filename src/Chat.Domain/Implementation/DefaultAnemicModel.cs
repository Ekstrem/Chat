using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Implementation;

/// <summary>
/// Пустая модель по умолчанию: используется провайдером агрегата, когда сообщения ещё нет.
/// </summary>
internal sealed class DefaultAnemicModel : IChatAnemicModel
{
    public Guid Id { get; init; }

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
        => new Dictionary<string, IValueObject>(StringComparer.Ordinal);

    /// <summary>Создать пустую модель по умолчанию с заданным идентификатором.</summary>
    public static DefaultAnemicModel Create(Guid id) => new() { Id = id };
}
