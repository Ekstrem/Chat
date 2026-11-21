using System.Security.Cryptography;
using System.Text;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.InternalContracts;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.Storage;

/// <summary>
/// РџСЂРѕРµРєС†РёРё РєР°РЅР°Р»Р°: СЃС‚СЂРѕРёС‚ MV <c>messages</c>, <c>inbox</c>, <c>last_read</c> РёР· РґРѕРјРµРЅРЅС‹С… СЃРѕР±С‹С‚РёР№.
/// РРґРµРјРїРѕС‚РµРЅС‚РµРЅ: СЃРѕР±С‹С‚РёРµ СЃ РІРµСЂСЃРёРµР№ РЅРµ Р±РѕР»СЊС€Рµ СѓР¶Рµ СЃРїСЂРѕРµС†РёСЂРѕРІР°РЅРЅРѕР№ РїСЂРѕРїСѓСЃРєР°РµС‚СЃСЏ.
/// </summary>
public sealed class MessageEventHandler : IDomainEventHandler<IChat>, IObserver<IDomainEvent<IChat>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MessageEventHandler> _logger;

    public MessageEventHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<MessageEventHandler> logger)
    {
        _scopeFactory = scopeFactory;
                _logger = logger;
    }

    public void OnCompleted() { }

    public void OnError(Exception error)
        => _logger.LogError(error, "РћС€РёР±РєР° РІ РїРѕС‚РѕРєРµ РґРѕРјРµРЅРЅС‹С… СЃРѕР±С‹С‚РёР№ Chat.");

    public void OnNext(IDomainEvent<IChat> domainEvent) => _ = ProjectAsync(domainEvent);

    /// <summary>РЎРїСЂРѕРµС†РёСЂРѕРІР°С‚СЊ СЃРѕР±С‹С‚РёРµ РІ Read Store.</summary>
    public async Task ProjectAsync(IDomainEvent<IChat> domainEvent)
    {
        var stage = "start";
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            stage = "resolve";
            var messageStore = provider.GetRequiredService<IReadModelStore<IChat, MessageReadModel>>();
            var messageReader = provider.GetRequiredService<IReadRepository<IChat, MessageReadModel>>();
            var readDb = provider.GetRequiredService<ReadDbContext>();

            stage = "load";
            var commandName = domainEvent.Command.CommandName;
            var version = domainEvent.Command.Version;
            var existing = await messageReader.GetByIdAsync(domainEvent.Id, CancellationToken.None).ConfigureAwait(false);

            if (existing is not null && existing.Version >= version)
            {
                return;
            }

            stage = "build";
            var row = BuildMessage(domainEvent, existing);
            stage = "upsert";
            await messageStore.UpsertAsync(row, CancellationToken.None).ConfigureAwait(false);
            stage = "cache";

            if (string.Equals(commandName, "SendMessage", StringComparison.Ordinal))
            {
                await ProjectInboxAsync(readDb, row).ConfigureAwait(false);
            }

            if (string.Equals(commandName, "ReadMessage", StringComparison.Ordinal))
            {
                await ProjectReadReceiptAsync(readDb, row).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "РќРµ СѓРґР°Р»РѕСЃСЊ СЃРїСЂРѕРµС†РёСЂРѕРІР°С‚СЊ СЃРѕР±С‹С‚РёРµ {CommandName} Р°РіСЂРµРіР°С‚Р° {AggregateId} РЅР° СЌС‚Р°РїРµ {Stage}.",
                domainEvent.Command.CommandName,
                domainEvent.Id,
                stage);
        }
    }

    private static MessageReadModel BuildMessage(IDomainEvent<IChat> domainEvent, MessageReadModel? existing)
    {
        var values = domainEvent.ChangedValueObjects;
        var root = values.TryGetValue("Root", out var rootValue) ? rootValue as IMessageRoot : null;
        var text = values.TryGetValue("Text", out var textValue) ? textValue as IMessageText : null;
        var addressee = values.TryGetValue("To", out var toValue) ? toValue as IAddressee : null;
        var attachments = values.TryGetValue("Attachments", out var attachmentValue) ? attachmentValue as IAttachmentList : null;
        var extended = values.TryGetValue("ExtendedFields", out var extendedValue) ? extendedValue as IExtendedFields : null;
        var receipts = values.TryGetValue("ReadReceipts", out var receiptsValue)
            ? (receiptsValue as IReadReceipts)?.Items
            : null;

        var now = DateTimeOffset.UtcNow.ToString("o");

        return new MessageReadModel
        {
            Id = domainEvent.Id,
            Product = root?.Product ?? existing?.Product ?? string.Empty,
            Role = root?.Role ?? existing?.Role ?? string.Empty,
            Timestamp = root?.Timestamp ?? existing?.Timestamp ?? domainEvent.Command.Version,
            To = addressee?.To.ToList() ?? existing?.To ?? new List<string>(),
            Text = text?.Value ?? existing?.Text ?? string.Empty,
            Attachments = attachments?.Items.ToList() ?? existing?.Attachments ?? new List<Guid>(),
            ReplyTo = existing?.ReplyTo,
            ExtendedFields = extended is null
                ? existing?.ExtendedFields ?? new Dictionary<string, string>()
                : new Dictionary<string, string>(extended.Items),
            ReadReceipts = receipts is null
                ? existing?.ReadReceipts ?? new List<ReadReceiptView>()
                : receipts.Select(receipt => new ReadReceiptView { Addressee = receipt.Addressee, Moment = receipt.Moment }).ToList(),
            Version = domainEvent.Command.Version,
            CreatedAt = existing?.CreatedAt ?? now,
            UpdatedAt = now
        };
    }

    private static async Task ProjectInboxAsync(ReadDbContext readDb, MessageReadModel row)
    {
        foreach (var addressee in row.To)
        {
            var id = DeterministicId.For("inbox", addressee, row.Id.ToString("N"));
            var existing = await readDb.Inbox
                .FirstOrDefaultAsync(item => item.Id == id)
                .ConfigureAwait(false);

            if (existing is null)
            {
                readDb.Inbox.Add(new InboxRowReadModel
                {
                    Id = id,
                    Role = addressee,
                    MessageId = row.Id,
                    From = row.Role,
                    Text = row.Text,
                    Timestamp = row.Timestamp,
                    ReadAt = null
                });
            }
            else
            {
                existing.From = row.Role;
                existing.Text = row.Text;
                existing.Timestamp = row.Timestamp;
            }
        }

        await readDb.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async Task ProjectReadReceiptAsync(ReadDbContext readDb, MessageReadModel row)
    {
        foreach (var receipt in row.ReadReceipts)
        {
            var id = DeterministicId.For("inbox", receipt.Addressee, row.Id.ToString("N"));
            var inboxRow = await readDb.Inbox
                .FirstOrDefaultAsync(item => item.Id == id)
                .ConfigureAwait(false);

            if (inboxRow is not null)
            {
                inboxRow.ReadAt = receipt.Moment;
            }

            var cursorId = DeterministicId.For("last_read", receipt.Addressee);
            var cursor = await readDb.LastRead
                .FirstOrDefaultAsync(item => item.Id == cursorId)
                .ConfigureAwait(false);

            if (cursor is null)
            {
                readDb.LastRead.Add(new LastReadReadModel
                {
                    Id = cursorId,
                    Role = receipt.Addressee,
                    LastReadAt = receipt.Moment
                });
            }
            else if (cursor.LastReadAt is null || cursor.LastReadAt < receipt.Moment)
            {
                cursor.LastReadAt = receipt.Moment;
            }
        }

        await readDb.SaveChangesAsync().ConfigureAwait(false);
    }
}

/// <summary>Р”РµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅС‹Рµ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂС‹ СЃС‚СЂРѕРє РїСЂРѕРµРєС†РёР№.</summary>
internal static class DeterministicId
{
    public static Guid For(params string[] parts)
    {
        var payload = Encoding.UTF8.GetBytes(string.Join("|", parts));
        var hash = MD5.HashData(payload);
        return new Guid(hash);
    }
}
