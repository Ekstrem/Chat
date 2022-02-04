using System;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction
{
    public interface IChatMessage: IValueObject
    {
        /// <summary>
        /// Тип сообщения (текст, речь, видео)
        /// </summary>
        MessageType Type { get; }

        /// <summary>
        /// Текст обращения
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Платформа с которой отправлено сообщение (Linux, Windows, Android)
        /// </summary>
        Platform Platform { get; }

        /// <summary>
        /// Приложение из которого отправлено сообщение (приложение, сайт, звонок)
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Идентификатор загруженного контента.
        /// </summary>
        Guid ContentId { get; }
    }
}
