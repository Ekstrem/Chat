using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.Monads;
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
            => ((await _repository.GetById(id, CancellationToken.None))?.FirstOrDefault() ?? DefaultAnemicModel.Create(id))
                .PipeTo(DecorateModel);

        public async Task<IChatAggregate> GetAggregateAsync(Guid id, long version, CancellationToken cancellationToken)
            => (await _repository.GetByIdAndVersion(id, version, CancellationToken.None) ?? DefaultAnemicModel.Create(id))
                .PipeTo(DecorateModel);

        public IChatAggregate GetAggregate(Guid id, long version)
            => GetAggregateAsync(id, version, CancellationToken.None)
                .GetAwaiter()
                .GetResult();

        public IChatAggregate GetAggregate(Guid id)
            => GetAggregateAsync(id, CancellationToken.None).Result;

        private IChatAggregate DecorateModel(IChatAnemicModel model)
            => model
                .PipeTo(Aggregate.Create)
                .PipeNotifierToContract(_observers);
    }
}
