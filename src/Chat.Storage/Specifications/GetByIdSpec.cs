using System;
using Ardalis.Specification;

namespace Chat.Storage.Specifications
{
    public class GetByIdSpec : Specification<DomainEventEntry>
    {
        public GetByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(t => t.AggregateId == id);
        }
    }
}
