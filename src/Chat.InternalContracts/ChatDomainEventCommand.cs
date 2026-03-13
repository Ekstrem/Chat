using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.TacticalPatterns;
using System;
using System.Collections.Generic;

namespace Chat.InternalContracts
{
    /// <summary>
    /// DTO для команды доменного события, поступающей из шины.
    /// </summary>
    public class ChatDomainEventCommand
    {
        public Guid AggregateId { get; set; }

        public ICommandToAggregate Command { get; set; }

        public IDictionary<string, IValueObject> ChangedValueObjects { get; set; }
    }
}
