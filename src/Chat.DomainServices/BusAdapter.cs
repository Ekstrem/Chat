using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using Microsoft.Extensions.Logging;

namespace Chat.DomainServices;

/// <summary>
/// Адаптер шины: единственный наблюдатель агрегата. Сначала фиксирует событие в Event Store,
/// затем публикует его в шину, поэтому следующая команда гарантированно видит предыдущую.
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
        var entry = DomainEventEntryFactory.Create(value);

        SaveEvent(entry);
        _eventBus.Publish(value.Event);
    }

    private void SaveEvent(DomainEventEntry entry)
    {
        try
        {
            // IObserver.OnNext синхронный, а порт Event Store — асинхронный: ждём фиксацию,
            // чтобы команда не вернула успех до записи события.
#pragma warning disable QG612
            _eventStore.SaveEventAsync(entry, CancellationToken.None).GetAwaiter().GetResult();
#pragma warning restore QG612
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Не удалось сохранить событие {CommandName} агрегата {AggregateId}.",
                entry.CommandName,
                entry.Id);
            throw;
        }
    }
}
