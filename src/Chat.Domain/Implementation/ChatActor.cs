using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation
{
    public class ChatActor: IChatActor
    {
        private ChatActor(string login, UserType type)
        {
            Login = login;
            Type = type;
        }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Login { get; }

        /// <summary>
        /// Тип пользователя (клиент, бот, оператор)
        /// </summary>
        public UserType Type { get; }

        public static ChatActor CreateInstance(string login, UserType type) 
            => new (login, type);
    }
}
