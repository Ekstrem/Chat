using System;
using System.Collections.Generic;
using Chat.InternalContracts;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.Storage
{
    public class DomainEventEntry
    {
        public Guid Id { get; set; }

        public Guid AggregateId { get; set; }

        public long AggregateVersion { get; set; }

        public long Version { get; set; }

        public Guid CorrelationToken { get; set; }

        public string CommandName { get; set; }

        public string SubjectName { get; set; }

        public IDictionary<string, IValueObject> ChangedValueObjects
            => ValueObjects
                .ToAnemicModel(AggregateId, AggregateVersion, CorrelationToken, CommandName, SubjectName)
                .GetValueObjects();

        public ICommandToAggregate Command
            => CommandToAggregate.Commit(CorrelationToken, CommandName, SubjectName, Version);

        public string ValueObjects { get; set; }

        public DomainOperationResultEnum Result { get; set; }

        public string Reason { get; set; }
    }
}
