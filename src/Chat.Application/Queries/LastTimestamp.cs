using Chat.Domain;
using Chat.InternalContracts;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using MediatR;

namespace Chat.Application.Queries;

/// <summary>Запрос LastTimestamp: последняя метка чата (монотонность, производная MAX).</summary>
public sealed record LastTimestampQuery : IRequest<long>;

/// <summary>Обработчик LastTimestamp: производная MAX(messages.timestamp) из Read Store.</summary>
public sealed class LastTimestampHandler : IQueryHandler, IRequestHandler<LastTimestampQuery, long>
{
    private readonly IQueryableReadRepository<IChat, MessageReadModel> _messages;

    public LastTimestampHandler(IQueryableReadRepository<IChat, MessageReadModel> messages) => _messages = messages;

    public Task<long> Handle(LastTimestampQuery request, CancellationToken cancellationToken)
    {
        var query = _messages.GetQueryable();
        var value = query.Any() ? query.Max(row => row.Timestamp) : 0L;
        return Task.FromResult(value);
    }
}
