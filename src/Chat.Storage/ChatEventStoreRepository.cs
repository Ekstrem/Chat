using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Storage;

/// <summary>
/// Event Store канала: наследует библиотечный Full Event Sourcing репозиторий и переопределяет
/// чтение, которое в пакете оставлено заглушкой, — состояние агрегата восстанавливается из журнала событий.
/// </summary>
public sealed class ChatEventStoreRepository : FullEventSourcingRepository<IChat, IChatAnemicModel>
{
    private readonly EventStoreDbContext _dbContext;

    public ChatEventStoreRepository(
        EventStoreDbContext dbContext,
        ILogger<FullEventSourcingRepository<IChat, IChatAnemicModel>> logger)
        : base(dbContext, logger)
        => _dbContext = dbContext;

    /// <inheritdoc />
    public override async Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var events = await LoadEvents(id, null, cancellationToken).ConfigureAwait(false);

        return events.Count == 0
            ? new List<IChatAnemicModel>()
            : new List<IChatAnemicModel> { MessageEventSerializer.Rehydrate(events) };
    }

    /// <inheritdoc />
    public override async Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
    {
        var events = await LoadEvents(id, version, cancellationToken).ConfigureAwait(false);

        return events.Count == 0 ? null! : MessageEventSerializer.Rehydrate(events);
    }

    /// <inheritdoc />
    public override async Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
    {
        var entry = await _dbContext.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.CorrelationToken == correlationToken, cancellationToken)
            .ConfigureAwait(false);

        if (entry is null)
        {
            return null!;
        }

        var events = await LoadEvents(entry.Id, entry.Version, cancellationToken).ConfigureAwait(false);

        return events.Count == 0 ? null! : MessageEventSerializer.Rehydrate(events);
    }

    private async Task<List<DigiTFactory.Libraries.CommandRepository.Postgres.Entities.DomainEventEntry>> LoadEvents(
        Guid id,
        long? version,
        CancellationToken cancellationToken)
        => await _dbContext.Events
            .AsNoTracking()
            .Where(item => item.Id == id && (version == null || item.Version <= version))
            .OrderBy(item => item.Version)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
