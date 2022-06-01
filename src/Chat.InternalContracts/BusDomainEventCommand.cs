using System;
using System.Collections.Generic;
using Chat.InternalContracts.Messaging;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Definition;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.InternalContracts
{
    public class BusDomainEventCommand<T> : IDomainEvent<T>, IMessage
        where T : IBoundedContext
    {
        private readonly IDomainEvent<T> _domainEvent;

        public BusDomainEventCommand(IDomainEvent<T> domainEvent)
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
