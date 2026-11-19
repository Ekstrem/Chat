using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>
/// Расширяемые поля канала. Канал хранит и передаёт словарь, но не интерпретирует ключи:
/// семантику (например priority/deadline) задаёт контекст «Агентская коммуникация».
/// </summary>
public interface IExtendedFields : IValueObject
{
    /// <summary>Значения расширяемых полей.</summary>
    IReadOnlyDictionary<string, string> Items { get; }
}
