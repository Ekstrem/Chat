using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction;

/// <summary>Набор вложений сообщения.</summary>
public interface IAttachmentList : IValueObject
{
    /// <summary>Идентификаторы вложений.</summary>
    IReadOnlyList<Guid> Items { get; }
}
