using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.Storage.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Storage
{
    internal sealed class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly CommandDbContext _dbContext;
        private readonly ChatProjectionService _projectionService;

        public Repository(
            ILogger<Repository> logger,
            CommandDbContext dbContext,
            ChatProjectionService projectionService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _projectionService = projectionService;
        }

        public async Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting events for aggregate {AggregateId}", id);

            var events = await _dbContext.Events
                .Where(e => e.Id == id)
                .OrderBy(e => e.Version)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {EventCount} events for aggregate {AggregateId}", events.Count, id);

            // TODO: Реконструкция анемичных моделей из потока событий
            return new List<IChatAnemicModel>();
        }

        public async Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting event for aggregate {AggregateId} version {Version}", id, version);

            var entry = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.Version == version, cancellationToken);

            // TODO: Десериализация события в анемичную модель
            return default!;
        }

        public async Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting event by correlation token {CorrelationToken}", correlationToken);

            var entry = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.CorrelationToken == correlationToken, cancellationToken);

            // TODO: Десериализация события в анемичную модель
            return default!;
        }

        public async Task SaveEventAsync(DomainEventEventEntry entry, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Saving domain event for aggregate {AggregateId}, version {Version}, command {CommandName}",
                entry.Id, entry.Version, entry.CommandName);

            _dbContext.Events.Add(entry);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Обновление read model
            await _projectionService.ProjectAsync(entry, cancellationToken);

            _logger.LogDebug("Domain event saved successfully for aggregate {AggregateId}", entry.Id);
        }
    }
}
