using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using Microsoft.Extensions.Logging;

namespace Chat.DomainServices;

/// <summary>
/// Адаптер шины: единственный наблюдатель агрегата. Публикует доменное событие в шину
/// и складывает его в Event Store.
/// </summary>
internal sealed class BusAdapter : IObserver<AggregateResult<IChat, IChatAnemicModel>>
{
    private readonly IEventBus _eventBus;
    private readonly IEventStoreRepository<IChat, IChatAnemicModel> _eventStore;
    private readonly ILogger<BusAdapter> _logger;

    public BusAdapter(
        IEventBus eventBus,
        IEventStoreRepository<IChat, IChatAnemicModel> eventStore,
        ILogger<BusAdapter> logger)
    {
        _eventBus = eventBus;
        _eventStore = eventStore;
        _logger = logger;
    }

    public void OnCompleted() { }

    public void OnError(Exception error)
        => _logger.LogError(error, "Ошибка в потоке результатов агрегата Message.");

    public void OnNext(AggregateResult<IChat, IChatAnemicModel> value)
    {
        var domainEvent = value.Event;
        var entry = new DomainEventEntry
        {
            Id = domainEvent.Id,
            Version = value.BusinessOperationData.Model.Version,
            CorrelationToken = domainEvent.CorrelationToken,
            BoundedContext = domainEvent.ContextName,
            CommandName = domainEvent.Command.CommandName,
            SubjectName = domainEvent.Command.SubjectName,
            ChangedValueObjectsJson = ValueObjectJson.Serialize(domainEvent.ChangedValueObjects),
            Result = domainEvent.Result.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _eventBus.Publish(domainEvent);
        _ = SaveEventAsync(entry);
    }

    private async Task SaveEventAsync(DomainEventEntry entry)
    {
        try
        {
            await _eventStore.SaveEventAsync(entry, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Не удалось сохранить событие {CommandName} агрегата {AggregateId}.",
                entry.CommandName,
                entry.Id);
        }
    }
}
