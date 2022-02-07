using Hive.SeedWorks.TacticalPatterns;
using System;
using System.Collections.Generic;

namespace Chat.InternalContracts
{
    public class ChatDomainEventCommand
    {
        public IDictionary<string, IValueObject> ChangedValueObjects()
            => throw new NotImplementedException();
    }
}
