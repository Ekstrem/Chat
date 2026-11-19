using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>Набор отметок о прочтении как значение агрегата.</summary>
public interface IReadReceipts : IValueObject
{
    /// <summary>Отметки о прочтении.</summary>
    IReadOnlyList<IReadReceipt> Items { get; }
}
