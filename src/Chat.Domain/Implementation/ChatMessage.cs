using System;
using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation
{
    public class ChatMessage: IChatMessage
    {
        private ChatMessage(MessageType type, string text, Platform platform, Application application, Guid contentId)
        {
            Type = type;
            Text = text;
            Platform = platform;
            Application = application;
            ContentId = contentId;
        }

        /// <summary>
        /// Тип сообщения (текст, речь, видео)
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Текст обращения.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Платформа с которой отправлено сообщение (Linux, Windows, Android)
        /// </summary>
        public Platform Platform { get; }

        /// <summary>
        /// Приложение из которого отправлено сообщение (приложение, сайт, звонок)
        /// </summary>
        public Application Application { get; }

        /// <summary>
        /// Идентификатор загруженного контента.
        /// </summary>
        public Guid ContentId { get; }

        public static ChatMessage CreateInstance(MessageType type, string text, Platform platform, Application application, Guid contentId) 
            => new (type, text, platform, application, contentId);
    }
}
