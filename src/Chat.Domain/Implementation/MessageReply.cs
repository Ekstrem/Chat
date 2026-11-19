using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Implementation;

/// <summary>Ссылка на сообщение, на которое отвечаем (0..1).</summary>
internal sealed class MessageReply : IValueObject
{
    public MessageReply(Guid? to) => To = to;

    /// <summary>Идентификатор исходного сообщения.</summary>
    public Guid? To { get; }
}
