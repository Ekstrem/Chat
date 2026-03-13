using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using Chat.InternalContracts;
using Chat.Storage.Specifications;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.TacticalPatterns;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Chat.Storage
{
    public class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly IQueryRepository<DomainEventEventEntry> _eventRepository;
        private readonly CommandDbContext _dbContext;

        public Repository(
            ILogger<Repository> logger,
            IQueryRepository<DomainEventEventEntry> eventRepository,
            CommandDbContext dbContext)
        {
            _logger = logger;
            _eventRepository = eventRepository;
            _dbContext = dbContext;
        }

        public async Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var entries = await _eventRepository.ListAsync(new GetByIdSpec(id), cancellationToken);
            if (entries == null || entries.Count == 0)
                return new List<IChatAnemicModel>();

            return entries
                .Select(ToAnemicModel)
                .ToList();
        }

        public async Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
        {
            var entry = await _eventRepository.FindByAsync(new GetByIdAndVersionSpec(id, version), cancellationToken);
            return entry != null ? ToAnemicModel(entry) : DefaultAnemicModel.Create(id);
        }

        public async Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
        {
            var entry = await _eventRepository.FindByAsync(new GetByCorrelationTokenSpec(correlationToken), cancellationToken);
            return entry != null ? ToAnemicModel(entry) : null;
        }

        /// <summary>
        /// Сохранить доменное событие в хранилище.
        /// </summary>
        public async Task SaveEventAsync(IDomainEvent<IChat> domainEvent, CancellationToken cancellationToken = default)
        {
            var entry = new DomainEventEventEntry
            {
                Id = domainEvent.Id,
                Version = domainEvent.Version,
                CorrelationToken = domainEvent.Command.CorrelationToken,
                CommandName = domainEvent.Command.CommandName,
                SubjectName = domainEvent.Command.SubjectName,
                ChangedValueObjectsJson = JsonConvert.SerializeObject(domainEvent.ChangedValueObjects),
                BoundedContext = domainEvent.ContextName,
                Result = (int)domainEvent.Result,
                Reason = domainEvent.Reason != null ? string.Join("; ", domainEvent.Reason) : string.Empty,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _dbContext.Events.Add(entry);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Событие сохранено: Id={Id}, Version={Version}, Command={Command}",
                entry.Id, entry.Version, entry.CommandName);
        }

        private static IChatAnemicModel ToAnemicModel(DomainEventEventEntry entry)
        {
            var command = CommandToAggregate.Commit(
                entry.CorrelationToken,
                entry.CommandName,
                entry.SubjectName,
                entry.Version);

            return AnemicModel.Create(
                entry.Id,
                command,
                null,
                null,
                null,
                Enumerable.Empty<IChatMessage>());
        }
    }
}
