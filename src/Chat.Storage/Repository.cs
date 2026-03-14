using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using Microsoft.Extensions.Logging;

namespace Chat.Storage
{
    /// <summary>
    /// Chat-специфичный репозиторий.
    /// Делегирует хранение в IAnemicModelRepository (из SeedWorks),
    /// который реализуется конкретной СУБД-библиотекой (Postgres или Mongo).
    /// </summary>
    internal sealed class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly IAnemicModelRepository<IChat, IChatAnemicModel> _store;

        public Repository(
            ILogger<Repository> logger,
            IAnemicModelRepository<IChat, IChatAnemicModel> store)
        {
            _logger = logger;
            _store = store;
        }

        public Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
            => _store.GetById(id, cancellationToken);

        public Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
            => _store.GetByIdAndVersion(id, version, cancellationToken);

        public Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
            => _store.GetByCorrelationToken(correlationToken, cancellationToken);
    }
}
