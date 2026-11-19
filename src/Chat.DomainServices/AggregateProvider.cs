using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>
/// Провайдер агрегата Message: загружает состояние из репозитория анемичных моделей,
/// при отсутствии — пустая модель по умолчанию, затем декорирует агрегат наблюдателями.
/// </summary>
internal sealed class AggregateProvider : IAggregateProvider<IChat, IChatAggregate>
{
    private readonly IAnemicModelRepository<IChat, IChatAnemicModel> _repository;
    private readonly IReadOnlyList<IObserver<AggregateResult<IChat, IChatAnemicModel>>> _observers;

    public AggregateProvider(
        IAnemicModelRepository<IChat, IChatAnemicModel> repository,
        IEnumerable<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
    {
        _repository = repository;
        _observers = observers.ToList();
    }

    /// <inheritdoc />
    public IChatAggregate GetAggregate(Guid id)
        => throw new NotSupportedException("Используйте GetAggregateAsync: загрузка состояния асинхронная.");

    /// <inheritdoc />
    public IChatAggregate GetAggregate(Guid id, long version)
        => throw new NotSupportedException("Используйте GetAggregateAsync: загрузка состояния асинхронная.");

    /// <inheritdoc />
    public async Task<IChatAggregate> GetAggregateAsync(Guid id, CancellationToken cancellationToken)
    {
        var models = await _repository.GetById(id, cancellationToken).ConfigureAwait(false);
        var model = models is { Count: > 0 }
            ? models[^1]
            : DefaultAnemicModel.Create(id);

        return Decorate(model);
    }

    /// <inheritdoc />
    public async Task<IChatAggregate> GetAggregateAsync(Guid id, long version, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAndVersion(id, version, cancellationToken).ConfigureAwait(false)
                    ?? (IChatAnemicModel)DefaultAnemicModel.Create(id);

        return Decorate(model);
    }

    private IChatAggregate Decorate(IChatAnemicModel model)
        => Aggregate.CreateInstance(model).PipeNotifierToContract(_observers);
}
