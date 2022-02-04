using Hive.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction
{
    public interface IChatFeedback : IValueObject
    {
        /// <summary>
        /// Форма ответа (да/нет, баллы, свободная)
        /// </summary>
        AnswerType Type { get; }

        /// <summary>
        /// Значение
        /// </summary>
        byte Value { get; }

        /// <summary>
        /// Текст
        /// </summary>
        string Text { get; }
    }
}
