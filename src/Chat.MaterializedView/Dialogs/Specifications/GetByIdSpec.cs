using Ardalis.Specification;
using Chat.InternalContracts;
using System;

namespace Chat.MaterializedView.Dialogs.Specifications
{
    public class GetByIdSpec : Specification<DialogView>
    {
        public GetByIdSpec(Guid id) 
            => Query
                .AsNoTracking()
                .Where(t => t.AggregateId == id);
    }
}
