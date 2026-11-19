using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>Отметка о прочтении сообщения адресатом.</summary>
internal sealed class MessageReadReceipt : IReadReceipt
{
    public string Addressee { get; init; } = string.Empty;

    public long Moment { get; init; }

    private MessageReadReceipt() { }

    public static IReadReceipt CreateInstance(string addressee, long moment)
        => new MessageReadReceipt { Addressee = addressee, Moment = moment };
}
