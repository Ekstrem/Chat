﻿using Ardalis.Specification;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.InternalContracts
{
    public interface IQueryRepository<T> where T : class
    {
        Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FindByAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    }
}
