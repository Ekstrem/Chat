using Hive.SeedWorks.TacticalPatterns;
using System.Collections.Generic;

namespace Chat.Domain.Abstraction
{
    /// <summary>
    /// Анемичная модель агрегата.
    /// </summary>
    public interface IChatAnemicModel : IAnemicModel<IChat>
    {
        /// <summary>
        /// Корень агрегата.
        /// </summary>
        IChatRoot Root { get; }

        /// <summary>
        /// Сообщения.
        /// </summary>
        IEnumerable<IChatMessage> Messages { get; }

        /// <summary>
        /// Данные пользователя.
        /// </summary>
        IChatActor Actor { get; }

        /// <summary>
        /// Полученный ответ.
        /// </summary>
        IChatFeedback Feedback { get; }
    }
}
