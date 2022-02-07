using Chat.Domain;
using Chat.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using Hive.SeedWorks.Monads;
using System.Collections.Immutable;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;

namespace Chat.DomainServices
{
    internal static class NotifierExtension
    {
        public static IChatAggregate PipeNotifierToContract(
            this IChatAggregate aggregate, List<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
            => Notifier.Create(aggregate, observers);

        public static IChatAggregate PipeNotifierToContract(
            this IChatAggregate aggregate, params IObserver<AggregateResult<IChat, IChatAnemicModel>>[] observers)
            => Notifier.Create(aggregate, observers.ToList());

        public static IQueryable<IChatAggregate> PipeNotifierToContract(
            this IQueryable<IChatAggregate> aggregates, List<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
            => aggregates.Select(m => m.PipeNotifierToContract(observers));

        public static IDisposable Subscribe(this IChatAggregate aggregate, IObserver<IDomainEvent<IChat>> observer)
            => aggregate is Notifier contractDomainEventNotifier
                ? contractDomainEventNotifier.Subscribe(observer)
                : null;

        public static void Subscribe(this ImmutableList<IChatAggregate> aggregates, IObserver<IDomainEvent<IChat>> observer)
            => aggregates.ForEach(e => e.Do(f => Subscribe(f, observer)));
    }
}
