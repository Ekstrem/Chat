using Hive.SeedWorks.Definition;

namespace Chat.Domain
{
    /// <summary>
    /// Ограниченный контекст чата.
    /// </summary>
    public interface IChat : IBoundedContext { }

    /// <summary>
    /// Описание ограниченного контекта чата.
    /// </summary>
    public class ChatBoundedContextDescription : IBoundedContextDescription
    {
        /// <summary>
        /// Наименование.
        /// </summary>
        public string ContextName => "Chat";

        /// <summary>
        /// Версия микросервиса.
        /// </summary>
        public int MicroserviceVersion => 1;
    }
}
