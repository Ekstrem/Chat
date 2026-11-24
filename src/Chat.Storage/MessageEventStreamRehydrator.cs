using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;

namespace Chat.Storage;

/// <summary>Доменный replay-порт Full Event Sourcing для агрегата Message.</summary>
public sealed class MessageEventStreamRehydrator : IEventStreamRehydrator<IChat, IChatAnemicModel>
{
    public IReadOnlyList<IChatAnemicModel> Rehydrate(IReadOnlyList<DomainEventEntry> events)
        => events.Count == 0
            ? Array.Empty<IChatAnemicModel>()
            : new[] { MessageEventSerializer.Rehydrate(events) };
}
