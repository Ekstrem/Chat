using Hive.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction
{
    public interface IChatActor : IValueObject
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        string Login { get; }

        /// <summary>
        /// Тип пользователя (клиент, бот, оператор)
        /// </summary>
        UserType Type { get; }
    }
}
