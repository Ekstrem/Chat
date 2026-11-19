using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.InternalContracts;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using Microsoft.EntityFrameworkCore;

namespace Chat.Storage;

/// <summary>
/// Репозиторий анемичных моделей Message: читает состояние из согласованного кеша процесса,
/// затем из проекции <c>messages</c> (Read Store).
/// </summary>
public sealed class MessageStateRepository : IAnemicModelRepository<IChat, IChatAnemicModel>
{
    private readonly ReadDbContext _readDb;
    private readonly MessageStateCache _cache;

    public MessageStateRepository(ReadDbContext readDb, MessageStateCache cache)
    {
        _readDb = readDb;
        _cache = cache;
    }

    public async Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (_cache.TryGet(id, out var cached))
        {
            return new List<IChatAnemicModel> { cached };
        }

        var row = await _readDb.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (row is null)
        {
            return new List<IChatAnemicModel>();
        }

        var model = ToAnemicModel(row);
        _cache.Set(id, model);
        return new List<IChatAnemicModel> { model };
    }

    public async Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
    {
        var models = await GetById(id, cancellationToken).ConfigureAwait(false);
        return models.FirstOrDefault(model => model.Version == version)!;
    }

    public Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
        => Task.FromResult<IChatAnemicModel>(null!);

    /// <summary>Перевести строку проекции в анемичную модель.</summary>
    public static IChatAnemicModel ToAnemicModel(MessageReadModel row)
        => new AnemicModel
        {
            Id = row.Id,
            Version = row.Version,
            Root = MessageRoot.CreateInstance(row.Product, row.Role, row.Timestamp),
            Text = MessageText.CreateInstance(row.Text),
            Addressee = MessageAddressee.CreateInstance(string.Join(" + ", row.To)),
            Attachments = MessageAttachments.CreateInstance(row.Attachments),
            ReplyTo = row.ReplyTo,
            ExtendedFields = MessageExtendedFields.CreateInstance(row.ExtendedFields),
            ReadReceipts = row.ReadReceipts
                .Select(receipt => MessageReadReceipt.CreateInstance(receipt.Addressee, receipt.Moment))
                .ToList()
        };
}
