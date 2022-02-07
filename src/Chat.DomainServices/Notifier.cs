using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Monads;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;
using System;
using System.Collections.Generic;
using Hive.SeedWorks.Reactive;

namespace Chat.DomainServices
{
    internal class Notifier :
        DomainEventNotifier<IChat, IChatAggregate>,
        IChatAggregate,
        IObservable<AggregateResult<IChat, IChatAnemicModel>>
    {
        private readonly List<IObserver<AggregateResult<IChat, IChatAnemicModel>>> _observers;

        private Notifier(IChatAggregate aggregate, List<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
            : base(aggregate) => _observers = observers;

        #region Aggregate Tech Props

        public Guid Id => Aggregate.Id;

        public long Version => Aggregate.Version;

        public string CommandName => Aggregate.CommandName;

        public string SubjectName => Aggregate.SubjectName;

        public Guid CorrelationToken => Aggregate.CorrelationToken;

        public IDictionary<string, IValueObject> GetValueObjects()
            => Aggregate.GetValueObjects();

        #endregion

        public IChatRoot Root => Aggregate.Root;

        public IEnumerable<IChatMessage> Messages => Aggregate.Messages;

        public IChatActor Actor => Aggregate.Actor;

        public IChatFeedback Feedback => Aggregate.Feedback;

        public AggregateResult<IChat, IChatAnemicModel> BotRepliedToUser(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .BotRepliedToUser(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> OperatorClarifiedQuestion(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .OperatorClarifiedQuestion(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> OperatorDequeueRequest(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .OperatorDequeueRequest(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> OperatorDownloadMedia(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .OperatorDownloadMedia(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> OperatorRepliedToMessage(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .OperatorRepliedToMessage(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> SessionEndingByTrigger(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .SessionEndingByTrigger(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> SubscriberDownloadMedia(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .SubscriberDownloadMedia(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> SubscriberGaveFeedback(IChatAnemicModel model, ICommandToAggregate commandMetadata)
            => Aggregate
                .SubscriberGaveFeedback(model, commandMetadata)
                .Do(MatchAndPushNotification);

        public AggregateResult<IChat, IChatAnemicModel> SubscriberRequestQuestion(IChatAnemicModel model)
            => Aggregate
                .SubscriberRequestQuestion(model)
                .Do(MatchAndPushNotification);

        public IDisposable Subscribe(IObserver<AggregateResult<IChat, IChatAnemicModel>> observer)
            => Unsubscriber<AggregateResult<IChat, IChatAnemicModel>>.Subscribe(_observers, observer);

        private void WithObservablesDo(Action<IObserver<AggregateResult<IChat, IChatAnemicModel>>> action)
            => _observers.ForEach(x => action.Invoke(x));

        private void MatchAndPushNotification(AggregateResult<IChat, IChatAnemicModel> result)
        {
            WithObservablesDo(a => a.OnNext(result));
        }

        internal static IChatAggregate Create(
            IChatAggregate aggregate,
            List<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
            => new Notifier(aggregate, observers);
    }
}
