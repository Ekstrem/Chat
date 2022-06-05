using Chat.InternalContracts;
using Microsoft.EntityFrameworkCore;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Hive.SeedWorks.Monads;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Chat.MaterializedView
{
    // TODO: move to hive
    internal class QueryRepository<TModel> : IQueryRepository<TModel>
         where TModel : class
    {
        private readonly DbContext _context;

        public QueryRepository(DbContext context)
            => _context = context;

        async Task<int> IQueryRepository<TModel>.CountAsync(ISpecification<TModel> spec, CancellationToken cancellationToken)
            => await ApplySpecification(spec)
                .CountAsync(cancellationToken);

        async Task<TModel> IQueryRepository<TModel>.FindByAsync(ISpecification<TModel> spec, CancellationToken cancellationToken)
            => await ApplySpecification(spec)
                .SingleOrDefaultAsync(cancellationToken);

        async Task<List<TModel>> IQueryRepository<TModel>.ListAllAsync(CancellationToken cancellationToken)
            => await ApplySpecification(default)
                .ToListAsync(cancellationToken);

        async Task<List<TModel>> IQueryRepository<TModel>.ListAsync(ISpecification<TModel> spec, CancellationToken cancellationToken)
            => await ApplySpecification(spec)
                .ToListAsync(cancellationToken);

        private IQueryable<TModel> ApplySpecification(ISpecification<TModel> spec)
            => _context
                .Set<TModel>()
                .AsNoTracking()
                .Either(
                    c => c != null,
                    s => new SpecificationEvaluator().GetQuery(s, spec),
                    f => f);
    }
}
