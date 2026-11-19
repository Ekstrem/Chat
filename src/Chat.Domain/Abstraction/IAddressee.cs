using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>Адресаты сообщения: <c>@роль</c>, несколько через «+» или <c>ALL</c>.</summary>
public interface IAddressee : IValueObject
{
    /// <summary>Список адресатов.</summary>
    IReadOnlyList<string> To { get; }
}
