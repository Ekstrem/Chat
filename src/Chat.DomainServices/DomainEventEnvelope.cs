using System;
using System.Collections.Generic;

namespace Chat.DomainServices
{
    /// <summary>
    /// Сериализуемая обёртка доменного события для передачи через MassTransit/RabbitMQ.
    /// </summary>
    public class DomainEventEnvelope
    {
        public Guid AggregateId { get; set; }
        public long Version { get; set; }
        public Guid CorrelationToken { get; set; }
        public string BoundedContext { get; set; }
        public string CommandName { get; set; }
        public string SubjectName { get; set; }
        public string ChangedValueObjectsJson { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
