using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using Microsoft.Extensions.Logging;

namespace Chat.DomainServices;

/// <summary>
/// Адаптер шины: публикует результат агрегата и связывает синхронную бизнес-операцию
/// с асинхронным durable-publish через completion провайдера. В Event Store не пишет.
/// </summary>
internal sealed class BusAdapter : IObserver<AggregateResult<IChat, IChatAnemicModel>>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<BusAdapter> _logger;
    private TaskCompletionSource? _completion;

    public BusAdapter(IEventBus eventBus, ILogger<BusAdapter> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>Привязать задачу завершения текущей бизнес-операции.</summary>
    public void UseCompletion(TaskCompletionSource completion) => _completion = completion;

    public void OnCompleted() { }

    public void OnError(Exception error)
        => _logger.LogError(error, "Ошибка в потоке результатов агрегата Message.");

    public void OnNext(AggregateResult<IChat, IChatAnemicModel> value)
    {
        if (value.Result != DomainOperationResultEnum.Success)
        {
            _completion?.TrySetResult();
            return;
        }

        _ = PublishAsync(value.Event, _completion);
    }

    private async Task PublishAsync(IDomainEvent<IChat> domainEvent, TaskCompletionSource? completion)
    {
        try
        {
            await _eventBus.PublishAsync(domainEvent).ConfigureAwait(false);
            completion?.TrySetResult();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Не удалось опубликовать событие {CommandName} агрегата {AggregateId}.",
                domainEvent.Command.CommandName,
                domainEvent.Id);
            completion?.TrySetException(exception);
        }
    }
}
