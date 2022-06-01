using System;
using Ardalis.Specification;

namespace Chat.Storage.Specifications
{
    public class GetByIdAndVersionSpec : Specification<DomainEventEntry>
    {
        public GetByIdAndVersionSpec(Guid id, long version)
        {
            Query
                .AsNoTracking()
                .Where(t => t.AggregateId == id && t.Version == version);
        }
    }
}
