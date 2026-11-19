using DigiTFactory.Libraries.SeedWorks.Definition;

namespace Chat.Domain;

/// <summary>
/// Ограниченный контекст «Сообщения» (чат-канал).
/// </summary>
public interface IChat : IBoundedContext { }

/// <summary>
/// Описание ограниченного контекста «Сообщения».
/// </summary>
public sealed class ChatBoundedContextDescription : IBoundedContextDescription
{
    /// <summary>Наименование контекста.</summary>
    public string ContextName => "Chat";

    /// <summary>Версия микросервиса.</summary>
    public int MicroserviceVersion => 1;
}
