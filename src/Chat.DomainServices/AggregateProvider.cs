using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.Monads;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.DomainServices
{
    internal class AggregateProvider : IChatAggregateProvider
    {
        private readonly IRepository _repository;
        private readonly IObserver<AggregateResult<IChat, IChatAnemicModel>>[] _observers;

        public AggregateProvider(
            IRepository repository,
            params IObserver<AggregateResult<IChat, IChatAnemicModel>>[] observers)
        {
            _repository = repository;
            _observers = observers;
        }

        public async Task<IChatAggregate> GetAggregateAsync(Guid id, CancellationToken cancellationToken)
            => ((await _repository.GetById(id, cancellationToken))?.FirstOrDefault() ?? DefaultAnemicModel.Create(id))
                .PipeTo(DecorateModel);

        public async Task<IChatAggregate> GetAggregateAsync(Guid id, long version, CancellationToken cancellationToken)
            => (await _repository.GetByIdAndVersion(id, version, cancellationToken) ?? DefaultAnemicModel.Create(id))
                .PipeTo(DecorateModel);

        /// <summary>
        /// Синхронная обёртка для Kafka/sync-контекстов.
        /// Task.Run предотвращает deadlock, выполняя async-код в отдельном потоке ThreadPool,
        /// не захватывая SynchronizationContext вызывающего потока.
        /// </summary>
        public IChatAggregate GetAggregate(Guid id, long version)
            => Task.Run(() => GetAggregateAsync(id, version, CancellationToken.None)).GetAwaiter().GetResult();

        public IChatAggregate GetAggregate(Guid id)
            => Task.Run(() => GetAggregateAsync(id, CancellationToken.None)).GetAwaiter().GetResult();

        private IChatAggregate DecorateModel(IChatAnemicModel model)
            => model
                .PipeTo(Aggregate.Create)
                .PipeNotifierToContract(_observers);
    }
}
