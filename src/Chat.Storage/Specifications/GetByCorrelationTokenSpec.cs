﻿using Ardalis.Specification;
using System;

namespace Chat.Storage.Specifications
{
    public class GetByCorrelationTokenSpec : Specification<DomainEventEventEntry>
    {
        public GetByCorrelationTokenSpec(Guid correlationToken)
        {
            Query
                .AsNoTracking()
                .Where(t => t.CorrelationToken == correlationToken);
        }
    }
}
