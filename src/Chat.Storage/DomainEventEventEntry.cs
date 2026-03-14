using System;

namespace Chat.Storage
{
    public class DomainEventEventEntry
    {
        public Guid Id { get; set; }

        public long Version { get; set; }

        public Guid CorrelationToken { get; set; }

        public string BoundedContext { get; set; } = string.Empty;

        public string CommandName { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public string ChangedValueObjectsJson { get; set; } = string.Empty;

        public string Result { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
