using DigiTFactory.Libraries.SeedWorks.Characteristics;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>
/// Контракт агрегата Message: команды канала (запись).
/// </summary>
public interface IChatAggregate : IAggregate<IChat>
{
    /// <summary>Создать сообщение (SendMessage).</summary>
    AggregateResult<IChat, IChatAnemicModel> SendMessage(
        ICommandToAggregate command,
        string product,
        string role,
        string to,
        string text,
        IEnumerable<Guid>? attachments,
        Guid? replyTo,
        IReadOnlyDictionary<string, string>? extendedFields);

    /// <summary>Добавить вложение (AddAttachment).</summary>
    AggregateResult<IChat, IChatAnemicModel> AddAttachment(ICommandToAggregate command, Guid attachmentId);

    /// <summary>Отметить сообщение прочитанным (ReadMessage).</summary>
    AggregateResult<IChat, IChatAnemicModel> ReadMessage(ICommandToAggregate command, string addressee);
}
