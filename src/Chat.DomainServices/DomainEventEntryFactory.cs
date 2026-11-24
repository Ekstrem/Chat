using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.DomainServices;

/// <summary>Преобразование доменного события в запись журнала Event Store.</summary>
public static class DomainEventEntryFactory
{
    /// <summary>Создать запись Event Store из результата агрегата.</summary>
    public static DomainEventEntry Create(AggregateResult<IChat, IChatAnemicModel> value)
        => Create(value.Event);

    /// <summary>Создать запись Event Store из опубликованного доменного события.</summary>
    public static DomainEventEntry Create(IDomainEvent<IChat> domainEvent)
        => new()
        {
            Id = domainEvent.Id,
            Version = domainEvent.Command.Version,
            CorrelationToken = domainEvent.CorrelationToken,
            BoundedContext = domainEvent.ContextName,
            CommandName = domainEvent.Command.CommandName,
            SubjectName = domainEvent.Command.SubjectName,
            ChangedValueObjectsJson = ValueObjectJson.Serialize(domainEvent.ChangedValueObjects),
            Result = domainEvent.Result.ToString(),
            CreatedAt = DateTime.UtcNow
        };
}
