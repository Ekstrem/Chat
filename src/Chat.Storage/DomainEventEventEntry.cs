using System;

namespace Chat.Storage
{
    public class DomainEventEventEntry
    {
        public Guid Id { get; set; }

        public long Version { get; set; }

        public Guid CorrelationToken { get; set; }

        public string CommandName { get; set; }

        public string SubjectName { get; set; }

        /// <summary>
        /// Сериализованные ChangedValueObjects (JSON).
        /// </summary>
        public string ChangedValueObjectsJson { get; set; }

        public string BoundedContext { get; set; }

        public int Result { get; set; }

        public string Reason { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
