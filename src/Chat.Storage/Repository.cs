using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.Storage.Projections;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using Microsoft.Extensions.Logging;

namespace Chat.Storage
{
    /// <summary>
    /// Chat-специфичный репозиторий.
    /// Делегирует хранение событий в библиотеку CommandRepository.Postgres,
    /// добавляет обновление Chat read model через ChatProjectionService.
    /// </summary>
    internal sealed class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly IEventStoreRepository<IChat, IChatAnemicModel> _eventStore;
        private readonly ChatProjectionService _projectionService;

        public Repository(
            ILogger<Repository> logger,
            IEventStoreRepository<IChat, IChatAnemicModel> eventStore,
            ChatProjectionService projectionService)
        {
            _logger = logger;
            _eventStore = eventStore;
            _projectionService = projectionService;
        }

        public Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
            => _eventStore.GetById(id, cancellationToken);

        public Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
            => _eventStore.GetByIdAndVersion(id, version, cancellationToken);

        public Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
            => _eventStore.GetByCorrelationToken(correlationToken, cancellationToken);

        public async Task SaveEventAsync(DomainEventEntry entry, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Saving domain event for aggregate {AggregateId}, version {Version}, command {CommandName}",
                entry.Id, entry.Version, entry.CommandName);

            await _eventStore.SaveEventAsync(entry, cancellationToken);

            // Chat-специфичное обновление read model
            await _projectionService.ProjectAsync(entry, cancellationToken);

            _logger.LogDebug("Domain event saved successfully for aggregate {AggregateId}", entry.Id);
        }
    }
}
