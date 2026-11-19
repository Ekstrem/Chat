using Chat.Domain;
using Chat.InternalContracts;
using Chat.InternalContracts.Views;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using MediatR;

namespace Chat.Application.Queries;

/// <summary>Запрос Search: поиск реплики по тексту, адресату или автору.</summary>
public sealed record SearchQuery(string? Text = null, string? To = null, string? From = null, int Limit = 100)
    : IRequest<IReadOnlyList<MessageView>>;

/// <summary>Обработчик Search: читает только Read Store (MV messages).</summary>
public sealed class SearchHandler : IQueryHandler, IRequestHandler<SearchQuery, IReadOnlyList<MessageView>>
{
    private readonly IQueryableReadRepository<IChat, MessageReadModel> _messages;

    public SearchHandler(IQueryableReadRepository<IChat, MessageReadModel> messages) => _messages = messages;

    public Task<IReadOnlyList<MessageView>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var query = _messages.GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Text))
        {
            query = query.Where(row => row.Text.Contains(request.Text));
        }

        if (!string.IsNullOrWhiteSpace(request.To))
        {
            query = query.Where(row => row.To.Contains(request.To));
        }

        if (!string.IsNullOrWhiteSpace(request.From))
        {
            query = query.Where(row => row.Role == request.From);
        }

        var rows = query
            .OrderByDescending(row => row.Timestamp)
            .Take(request.Limit <= 0 ? 100 : request.Limit)
            .ToList();

        IReadOnlyList<MessageView> views = rows.Select(row => new MessageView
        {
            Id = row.Id,
            Product = row.Product,
            Role = row.Role,
            Timestamp = row.Timestamp,
            To = row.To,
            Text = row.Text,
            Attachments = row.Attachments,
            ReplyTo = row.ReplyTo,
            ExtendedFields = row.ExtendedFields,
            ReadReceipts = row.ReadReceipts
                .Select(receipt => new ReadReceiptItemView { Addressee = receipt.Addressee, Moment = receipt.Moment })
                .ToList()
        }).ToList();

        return Task.FromResult(views);
    }
}
