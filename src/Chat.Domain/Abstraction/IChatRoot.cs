using Hive.SeedWorks.TacticalPatterns;
using System;

namespace Chat.Domain.Abstraction
{
    public interface IChatRoot : IAggregateRoot<IChat>
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// Идентификатор сессии.
        /// </summary>
        int SessionId { get; }
    }
}
