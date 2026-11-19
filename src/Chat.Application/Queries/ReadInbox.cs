using Chat.Domain;
using Chat.InternalContracts;
using Chat.InternalContracts.Views;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using MediatR;

namespace Chat.Application.Queries;

/// <summary>Запрос ReadInbox: только адресованное роли и новее её курсора (MCP read_inbox).</summary>
public sealed record ReadInboxQuery(string Role) : IRequest<InboxView>;

/// <summary>Обработчик ReadInbox: читает только Read Store (MV inbox + last_read).</summary>
public sealed class ReadInboxHandler : IQueryHandler, IRequestHandler<ReadInboxQuery, InboxView>
{
    private readonly IQueryableReadRepository<IChat, InboxRowReadModel> _inbox;
    private readonly IQueryableReadRepository<IChat, LastReadReadModel> _cursors;

    public ReadInboxHandler(
        IQueryableReadRepository<IChat, InboxRowReadModel> inbox,
        IQueryableReadRepository<IChat, LastReadReadModel> cursors)
    {
        _inbox = inbox;
        _cursors = cursors;
    }

    public Task<InboxView> Handle(ReadInboxQuery request, CancellationToken cancellationToken)
    {
        var cursor = _cursors.GetQueryable()
            .Where(row => row.Role == request.Role)
            .Select(row => row.LastReadAt)
            .FirstOrDefault();

        var rows = _inbox.GetQueryable()
            .Where(row => row.Role == request.Role && (cursor == null || row.Timestamp > cursor))
            .OrderBy(row => row.Timestamp)
            .ToList();

        var view = new InboxView
        {
            Role = request.Role,
            Since = cursor,
            Messages = rows.Select(row => new InboxItemView
            {
                MessageId = row.MessageId,
                From = row.From,
                Timestamp = row.Timestamp,
                Text = row.Text
            }).ToList()
        };

        return Task.FromResult(view);
    }
}
