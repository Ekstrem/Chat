using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.DomainServices;

/// <summary>Преобразование результата бизнес-операции в запись журнала событий.</summary>
public static class DomainEventEntryFactory
{
    /// <summary>Создать запись Event Store из результата агрегата.</summary>
    public static DomainEventEntry Create(AggregateResult<IChat, IChatAnemicModel> value)
    {
        var domainEvent = value.Event;

        return new DomainEventEntry
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
    }
}
