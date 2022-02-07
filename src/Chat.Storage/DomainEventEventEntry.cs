using System;

namespace Chat.Storage
{
    public class DomainEventEventEntry
    {
        public Guid Id { get; set; }

        public long Version { get; set; }

        public Guid CorrelationToken { get; set; }
    }
}
