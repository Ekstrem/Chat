using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>
/// Провайдер агрегата Message: загружает состояние из Event Store, декорирует агрегат
/// наблюдателями и возвращает completion, завершающийся после durable-publish.
/// </summary>
internal sealed class AggregateProvider : IAggregateProviderWithCompletion<IChat, IChatAggregate>
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
        => throw new NotSupportedException("Используйте GetAggregateWithCompletionAsync: загрузка состояния асинхронная.");

    /// <inheritdoc />
    public IChatAggregate GetAggregate(Guid id, long version)
        => throw new NotSupportedException("Используйте GetAggregateWithCompletionAsync: загрузка состояния асинхронная.");

    /// <inheritdoc />
    public async Task<IChatAggregate> GetAggregateAsync(Guid id, CancellationToken cancellationToken)
        => (await GetAggregateWithCompletionAsync(id, cancellationToken).ConfigureAwait(false)).Aggregate;

    /// <inheritdoc />
    public async Task<IChatAggregate> GetAggregateAsync(Guid id, long version, CancellationToken cancellationToken)
        => (await GetAggregateWithCompletionAsync(id, version, cancellationToken).ConfigureAwait(false)).Aggregate;

    /// <inheritdoc />
    public async Task<(IChatAggregate Aggregate, Task Completion)> GetAggregateWithCompletionAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var models = await _repository.GetById(id, cancellationToken).ConfigureAwait(false);
        var model = models is { Count: > 0 }
            ? models[^1]
            : DefaultAnemicModel.Create(id);

        return Decorate(model);
    }

    /// <inheritdoc />
    public async Task<(IChatAggregate Aggregate, Task Completion)> GetAggregateWithCompletionAsync(
        Guid id,
        long version,
        CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAndVersion(id, version, cancellationToken).ConfigureAwait(false)
                    ?? (IChatAnemicModel)DefaultAnemicModel.Create(id);

        return Decorate(model);
    }

    private (IChatAggregate Aggregate, Task Completion) Decorate(IChatAnemicModel model)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var busAdapter = _observers.OfType<BusAdapter>().FirstOrDefault()
                         ?? throw new InvalidOperationException("BusAdapter не зарегистрирован как наблюдатель агрегата.");
        busAdapter.UseCompletion(completion);

        var aggregate = Aggregate.CreateInstance(model).PipeNotifierToContract(_observers);
        return (aggregate, completion.Task);
    }
}
