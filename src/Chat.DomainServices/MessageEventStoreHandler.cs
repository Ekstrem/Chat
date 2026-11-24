using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using DigiTFactory.Libraries.SeedWorks.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.DomainServices;

/// <summary>
/// Durable-подписчик шины: сохраняет доменное событие в Event Store.
/// InMemory EventBus ожидает завершение этого обработчика до запуска проекций.
/// </summary>
public sealed class MessageEventStoreHandler : IAsyncDomainEventHandler<IChat>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageEventStoreHandler(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task HandleAsync(IDomainEvent<IChat> domainEvent)
    {
        using var scope = _scopeFactory.CreateScope();
        var eventStore = scope.ServiceProvider.GetRequiredService<IEventStoreRepository<IChat, IChatAnemicModel>>();
        var entry = DomainEventEntryFactory.Create(domainEvent);

        await eventStore.SaveEventAsync(entry, CancellationToken.None).ConfigureAwait(false);
    }
}
