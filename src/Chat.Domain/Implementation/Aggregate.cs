using Chat.Domain.Abstraction;
using System;
using System.Collections.Generic;
using Hive.SeedWorks.TacticalPatterns;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.Characteristics;
using Chat.Domain.Specifications;
using Hive.SeedWorks.Monads;
using System.Linq;

namespace Chat.Domain.Implementation
{
    public class Aggregate : IChatAggregate
    {

        private readonly IChatAnemicModel _anemicModel;

        private Aggregate(IChatAnemicModel anemicModel) => _anemicModel = anemicModel;

        public static Aggregate Create(IChatAnemicModel anemicModel) => new Aggregate(anemicModel);

        #region Aggregate tech props

        public Guid Id => _anemicModel.Id;

        public long Version => _anemicModel.Version;

        public string CommandName => _anemicModel.CommandName;

        public string SubjectName => _anemicModel.SubjectName;

        public Guid CorrelationToken => _anemicModel.CorrelationToken;

        public IDictionary<string, IValueObject> GetValueObjects()
            => _anemicModel.GetValueObjects();

        #endregion

        #region AnemicModel

        /// <summary>
        /// Корень агрегата.
        /// </summary>
        public IChatRoot Root => _anemicModel.Root;

        /// <summary>
        /// Данные пользователя.
        /// </summary>
        public IChatActor Actor => _anemicModel.Actor;

        /// <summary>
        /// Полученный ответ.
        /// </summary>
        public IChatFeedback Feedback => _anemicModel.Feedback;

        /// <summary>
        /// Сообщения.
        /// </summary>
        public IReadOnlyCollection<IChatMessage> Messages => _anemicModel.Messages;

        #endregion

        /// <summary>
        /// Абонент задал вопрос в чате.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> SubscriberRequestQuestion(IChatAnemicModel model)
            => BusinessOperationData
                .Commit(_anemicModel, model)
                .ValidateCommand(
                    new IsNewSubscriberRequestValidator())
                .PipeTo(r => new ChatResult(r));

        /// <summary>
        /// Бот ответил пользователю.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> BotRepliedToUser(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => AnemicModel
                .Create(
                    _anemicModel.Id, commandMetadata,
                    _anemicModel.Root, _anemicModel.Actor, _anemicModel.Feedback, new List<IChatMessage>(_anemicModel.Messages.Concat(model.Messages)))
                .PipeTo(am => new ChatResult(_anemicModel, am, DomainOperationResultEnum.Success, Enumerable.Empty<string>()));

        /// <summary>
        /// Оператор получил обращение на обработку.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> OperatorDequeueRequest(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => throw new NotImplementedException();

        /// <summary>
        /// Оператор ответил на обращение.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> OperatorRepliedToMessage(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => AnemicModel
                .Create(
                    _anemicModel.Id, commandMetadata,
                    _anemicModel.Root, _anemicModel.Actor, _anemicModel.Feedback, new List<IChatMessage>(_anemicModel.Messages.Concat(model.Messages)))
                .PipeTo(am => new ChatResult(_anemicModel, am, DomainOperationResultEnum.Success, Enumerable.Empty<string>()));

        /// <summary>
        /// Оператор уточник вопрос.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> OperatorClarifiedQuestion(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => AnemicModel
                .Create(
                    _anemicModel.Id, commandMetadata,
                    _anemicModel.Root, _anemicModel.Actor, _anemicModel.Feedback, new List<IChatMessage>(_anemicModel.Messages.Concat(model.Messages)))
                .PipeTo(am => new ChatResult(_anemicModel, am, DomainOperationResultEnum.Success, Enumerable.Empty<string>()));

        /// <summary>
        /// Абонент оценил работу.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> SubscriberGaveFeedback(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => AnemicModel
                .Create(
                    _anemicModel.Id, commandMetadata,
                    _anemicModel.Root, _anemicModel.Actor, model.Feedback, _anemicModel.Messages)
                .PipeTo(am => new ChatResult(_anemicModel, am, DomainOperationResultEnum.Success, Enumerable.Empty<string>()));

        /// <summary>
        /// Абонент загрузил медиа-контент.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> SubscriberDownloadMedia(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => throw new NotImplementedException();

        /// <summary>
        /// Оператор загрузил медиа-контент.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> OperatorDownloadMedia(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => throw new NotImplementedException();

        /// <summary>
        /// Сессия закончилась по триггеру.
        /// </summary>
        public AggregateResult<IChat, IChatAnemicModel> SessionEndingByTrigger(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => throw new NotImplementedException();
    }
}
