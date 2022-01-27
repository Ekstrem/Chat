using System;
using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation
{
    public class ChatRoot: IChatRoot
    {
        private ChatRoot(Guid userId, int sessionId)
        {
            UserId = userId;
            SessionId = sessionId;
        }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Идентификатор сессии.
        /// </summary>
        public int SessionId { get; }

        public static ChatRoot CreateInstance(Guid userId, int sessionId) 
            => new (userId, sessionId);
    }
}
