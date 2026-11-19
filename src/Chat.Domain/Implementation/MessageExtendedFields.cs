using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>Расширяемые поля канала: словарь хранится и передаётся как есть.</summary>
internal sealed class MessageExtendedFields : IExtendedFields
{
    public IReadOnlyDictionary<string, string> Items { get; init; } = new Dictionary<string, string>();

    private MessageExtendedFields() { }

    public static IExtendedFields CreateInstance(IReadOnlyDictionary<string, string>? items)
        => new MessageExtendedFields
        {
            Items = items is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(items, StringComparer.Ordinal)
        };

    public static IExtendedFields Empty() => new MessageExtendedFields { Items = new Dictionary<string, string>() };
}
