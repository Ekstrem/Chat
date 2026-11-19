using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>Корень агрегата Message: (product, role, timestamp).</summary>
internal sealed class MessageRoot : IMessageRoot
{
    public string Product { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public long Timestamp { get; init; }

    private MessageRoot() { }

    public static IMessageRoot CreateInstance(string product, string role, long timestamp)
        => new MessageRoot
        {
            Product = product ?? string.Empty,
            Role = role ?? string.Empty,
            Timestamp = timestamp
        };

    public static IMessageRoot Empty() => CreateInstance(string.Empty, string.Empty, 0);
}
