using Chat.Domain.Abstraction;
using System;

namespace Chat.Domain.Tests.Models
{
    public class Question
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Идентификатор сессии.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Текст входящего сообщения.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Платформа с которой отправлено сообщение (Linux, Windows, Android)
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        /// Приложение из которого отправлено сообщение (приложение, сайт, звонок)
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        /// Тип сообщения (текст, речь, видео)
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Идентификатор загруженного контента.
        /// </summary>
        public Guid ContentId { get; set; }
    }
}
