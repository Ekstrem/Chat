using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Abstraction
{
    /// <summary>
    /// Агрегат чата.
    /// </summary>
    public interface IChatAggregate :
        IAggregate<IChat>,
        IChatAnemicModel
    {
        /// <summary>
        /// Абонент задал вопрос в чате.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> SubscriberRequestQuestion(IChatAnemicModel model);

        /// <summary>
        /// Бот ответил пользователю.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> BotRepliedToUser(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Оператор получил обращение на обработку.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> OperatorDequeueRequest(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Оператор ответил на обращение.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> OperatorRepliedToMessage(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Оператор уточник вопрос.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> OperatorClarifiedQuestion(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Абонент оценил работу.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> SubscriberGaveFeedback(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Абонент загрузил медиа-контент.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> SubscriberDownloadMedia(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Оператор загрузил медиа-контент.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> OperatorDownloadMedia(IChatAnemicModel model, ICommandToAggregate commandMetadata);

        /// <summary>
        /// Сессия закончилась по триггеру.
        /// </summary>
        AggregateResult<IChat, IChatAnemicModel> SessionEndingByTrigger(IChatAnemicModel model, ICommandToAggregate commandMetadata);
    }
}
