using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>Набор вложений сообщения (0..n).</summary>
internal sealed class MessageAttachments : IAttachmentList
{
    public IReadOnlyList<Guid> Items { get; init; } = Array.Empty<Guid>();

    private MessageAttachments() { }

    public static IAttachmentList CreateInstance(IEnumerable<Guid>? items)
        => new MessageAttachments
        {
            Items = (items ?? Array.Empty<Guid>()).Distinct().ToList()
        };

    public static IAttachmentList Empty() => new MessageAttachments { Items = Array.Empty<Guid>() };
}
