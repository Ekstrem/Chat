using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>Набор отметок о прочтении (0..n).</summary>
internal sealed class MessageReceipts : IReadReceipts
{
    public MessageReceipts(IReadOnlyList<IReadReceipt> items) => Items = items;

    public IReadOnlyList<IReadReceipt> Items { get; }
}
