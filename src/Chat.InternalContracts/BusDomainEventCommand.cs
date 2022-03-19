using System;
using System.Collections.Generic;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Definition;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;
using MediatR;

namespace Chat.InternalContracts
{
    public class BusDomainEventCommand<T> : INotification, IDomainEvent<T>
        where T : IBoundedContext
    {
        private readonly IDomainEvent<IBoundedContext> _domainEvent;

        public BusDomainEventCommand(IDomainEvent<IBoundedContext> domainEvent)
        {
            _domainEvent = domainEvent;
        }

        public string ContextName => _domainEvent.ContextName;

        public int MicroserviceVersion => _domainEvent.MicroserviceVersion;

        public ICommandToAggregate Command => _domainEvent.Command;

        public IDictionary<string, IValueObject> ChangedValueObjects => _domainEvent.ChangedValueObjects;

        public DomainOperationResultEnum Result => _domainEvent.Result;

        public string Reason => _domainEvent.Reason;

        public Guid Id => _domainEvent.Id;

        public long Version => _domainEvent.Version;
    }
}
