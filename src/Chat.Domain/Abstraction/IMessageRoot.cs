using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>
/// Корень агрегата Message: доменная идентичность (product, role, timestamp).
/// </summary>
public interface IMessageRoot : IAggregateRoot<IChat>
{
    /// <summary>Проект (корень идентичности платформы).</summary>
    string Product { get; }

    /// <summary>Роль-автор сообщения.</summary>
    string Role { get; }

    /// <summary>Метка времени, Unix ms — она же Version агрегата.</summary>
    long Timestamp { get; }
}
