using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>Текст сообщения — неизменяемое значение.</summary>
public interface IMessageText : IValueObject
{
    /// <summary>Содержимое текста.</summary>
    string Value { get; }
}
