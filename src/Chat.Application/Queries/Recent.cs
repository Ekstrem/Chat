using Chat.Domain;
using Chat.InternalContracts;
using Chat.InternalContracts.Views;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using MediatR;

namespace Chat.Application.Queries;

/// <summary>Запрос Recent: сводка «что нового» после указанной метки.</summary>
public sealed record RecentQuery(long Since, int Limit = 200) : IRequest<SummaryView>;

/// <summary>Обработчик Recent: читает только Read Store (MV messages).</summary>
public sealed class RecentHandler : IQueryHandler, IRequestHandler<RecentQuery, SummaryView>
{
    private readonly IQueryableReadRepository<IChat, MessageReadModel> _messages;

    public RecentHandler(IQueryableReadRepository<IChat, MessageReadModel> messages) => _messages = messages;

    public Task<SummaryView> Handle(RecentQuery request, CancellationToken cancellationToken)
    {
        var rows = _messages.GetQueryable()
            .Where(row => row.Timestamp > request.Since)
            .OrderBy(row => row.Timestamp)
            .Take(request.Limit <= 0 ? 200 : request.Limit)
            .ToList();

        var view = new SummaryView
        {
            Since = request.Since,
            Messages = rows.Select(row => new SummaryItemView
            {
                MessageId = row.Id,
                From = row.Role,
                To = row.To,
                Timestamp = row.Timestamp,
                Text = row.Text
            }).ToList()
        };

        return Task.FromResult(view);
    }
}
