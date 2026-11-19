using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.EntityFrameworkCore;

namespace Chat.Storage;

/// <summary>
/// Queryable-доступ к Read Store поверх <see cref="ReadDbContext"/>.
/// Нужен потому, что библиотека регистрирует только <c>IReadRepository</c>,
/// а запросам требуется фильтрация по колонкам проекций.
/// </summary>
public sealed class ReadModelQuery<TReadModel> : IQueryableReadRepository<IChat, TReadModel>
    where TReadModel : class, IReadModel<IChat>
{
    private readonly ReadDbContext _dbContext;

    public ReadModelQuery(ReadDbContext dbContext) => _dbContext = dbContext;

    public IQueryable<TReadModel> GetQueryable() => _dbContext.Set<TReadModel>().AsQueryable();

    public async Task<TReadModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Set<TReadModel>()
            .FindAsync(new object[] { id }, cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<TReadModel>> GetAllAsync(IPaging paging, CancellationToken cancellationToken = default)
        => await _dbContext.Set<TReadModel>()
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Set<TReadModel>().LongCountAsync(cancellationToken).ConfigureAwait(false);
}
