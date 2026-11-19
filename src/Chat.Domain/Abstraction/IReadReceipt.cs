using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>Отметка о прочтении сообщения конкретным адресатом.</summary>
public interface IReadReceipt : IValueObject
{
    /// <summary>Адресат, прочитавший сообщение.</summary>
    string Addressee { get; }

    /// <summary>Момент прочтения, Unix ms.</summary>
    long Moment { get; }
}
