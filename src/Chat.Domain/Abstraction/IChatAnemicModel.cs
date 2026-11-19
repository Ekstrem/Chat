using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>
/// Анемичная модель сообщения: полное состояние агрегата Message.
/// </summary>
public interface IChatAnemicModel : IAnemicModel<IChat>
{
    /// <summary>Корень: product + role + timestamp (доменная идентичность).</summary>
    IMessageRoot Root { get; }

    /// <summary>Текст сообщения (UTF-8, ≤ 32 КБ).</summary>
    IMessageText Text { get; }

    /// <summary>Адресаты: один, несколько или ALL.</summary>
    IAddressee Addressee { get; }

    /// <summary>Вложения.</summary>
    IAttachmentList Attachments { get; }

    /// <summary>Идентификатор сообщения, на которое отвечаем.</summary>
    Guid? ReplyTo { get; }

    /// <summary>Расширяемые поля канала (семантику задаёт потребитель).</summary>
    IExtendedFields ExtendedFields { get; }

    /// <summary>Отметки о прочтении по каждому адресату.</summary>
    IReadOnlyList<IReadReceipt> ReadReceipts { get; }
}
