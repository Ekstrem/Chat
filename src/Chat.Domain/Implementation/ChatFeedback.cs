using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation
{
    public class ChatFeedback: IChatFeedback
    {
        private ChatFeedback(AnswerType type, byte value, string text)
        {
            Value = value;
            Type = type;
            Text = text;
        }

        /// <summary>
        /// Форма ответа (да/нет, баллы, свободная)
        /// </summary>
        public AnswerType Type { get; }

        /// <summary>
        /// Значение.
        /// </summary>
        public byte Value { get; }

        /// <summary>
        /// Текст.
        /// </summary>
        public string Text { get; }

        public static ChatFeedback CreateByText(string text) 
            => new (AnswerType.Free, default, text);

        public static ChatFeedback CreateByScores(byte value)
            => new(AnswerType.Scores, value, string.Empty);
    }
}
