using System;
using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Storage.ReadModel
{
    /// <summary>
    /// Материализованное представление чата (проекция).
    /// Денормализованная сущность, проецируемая из доменных событий.
    /// </summary>
    public class ChatReadModel : IReadModel<IChat>
    {
        /// <summary>
        /// Идентификатор агрегата (чата).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор сессии.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Логин последнего актора.
        /// </summary>
        public string ActorLogin { get; set; } = string.Empty;

        /// <summary>
        /// Тип последнего актора (Client, Bot, Operator).
        /// </summary>
        public string ActorType { get; set; } = string.Empty;

        /// <summary>
        /// Логин назначенного оператора.
        /// </summary>
        public string OperatorLogin { get; set; }

        /// <summary>
        /// Текст последнего сообщения.
        /// </summary>
        public string LastMessageText { get; set; }

        /// <summary>
        /// Количество сообщений в чате.
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Статус чата (New, InProgress, Closed).
        /// </summary>
        public string Status { get; set; } = "New";

        /// <summary>
        /// Текст обратной связи.
        /// </summary>
        public string FeedbackText { get; set; }

        /// <summary>
        /// Оценка обратной связи.
        /// </summary>
        public byte? FeedbackValue { get; set; }

        /// <summary>
        /// Последняя команда, изменившая агрегат.
        /// </summary>
        public string LastCommand { get; set; } = string.Empty;

        /// <summary>
        /// Версия агрегата.
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Время создания.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Время последнего обновления.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
