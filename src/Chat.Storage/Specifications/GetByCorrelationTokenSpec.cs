using Ardalis.Specification;
using System;

namespace Chat.Storage.Specifications
{
    public class GetByCorrelationTokenSpec : Specification<DomainEventEntry>
    {
        public GetByCorrelationTokenSpec(Guid correlationToken) 
            => Query
                .AsNoTracking()
                .Where(t => t.CorrelationToken == correlationToken);
    }
}
