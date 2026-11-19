using System.Text.Json;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;

namespace Chat.Storage;

/// <summary>
/// Восстановление состояния агрегата Message из журнала доменных событий.
/// Каждое событие несёт изменённые Value Object-ы; состояние складывается по версиям.
/// </summary>
public static class MessageEventSerializer
{
    /// <summary>Восстановить анемичную модель по упорядоченным по версии событиям агрегата.</summary>
    public static IChatAnemicModel Rehydrate(IReadOnlyList<DomainEventEntry> events)
    {
        if (events.Count == 0)
        {
            throw new ArgumentException("Для восстановления состояния нужен хотя бы один event.", nameof(events));
        }

        var product = string.Empty;
        var role = string.Empty;
        var timestamp = 0L;
        var text = string.Empty;
        var to = new List<string>();
        var attachments = new List<Guid>();
        Guid? replyTo = null;
        var extendedFields = new Dictionary<string, string>();
        var receipts = new List<IReadReceipt>();

        foreach (var entry in events.OrderBy(item => item.Version))
        {
            using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(entry.ChangedValueObjectsJson)
                ? "{}"
                : entry.ChangedValueObjectsJson);

            var root = document.RootElement;

            if (root.TryGetProperty("Root", out var rootValue))
            {
                product = ReadString(rootValue, "Product") ?? product;
                role = ReadString(rootValue, "Role") ?? role;
                timestamp = ReadLong(rootValue, "Timestamp") ?? timestamp;
            }

            if (root.TryGetProperty("Text", out var textValue) && ReadString(textValue, "Value") is { } newText)
            {
                text = newText;
            }

            if (root.TryGetProperty("To", out var toValue) && toValue.TryGetProperty("To", out var toArray))
            {
                to = toArray.EnumerateArray().Select(item => item.GetString() ?? string.Empty).ToList();
            }

            if (root.TryGetProperty("Attachments", out var attachmentsValue)
                && attachmentsValue.TryGetProperty("Items", out var attachmentsArray))
            {
                attachments = attachmentsArray.EnumerateArray()
                    .Select(item => item.TryGetGuid(out var id) ? id : Guid.Empty)
                    .Where(id => id != Guid.Empty)
                    .ToList();
            }

            if (root.TryGetProperty("Reply", out var replyValue)
                && replyValue.TryGetProperty("To", out var replyToValue))
            {
                replyTo = replyToValue.ValueKind == JsonValueKind.String && replyToValue.TryGetGuid(out var id) ? id : null;
            }

            if (root.TryGetProperty("ExtendedFields", out var extendedValue)
                && extendedValue.TryGetProperty("Items", out var extendedItems))
            {
                extendedFields = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var property in extendedItems.EnumerateObject())
                {
                    extendedFields[property.Name] = property.Value.GetString() ?? string.Empty;
                }
            }

            if (root.TryGetProperty("ReadReceipts", out var receiptsValue)
                && receiptsValue.TryGetProperty("Items", out var receiptsArray))
            {
                receipts = receiptsArray.EnumerateArray()
                    .Select(item => (IReadReceipt)MessageReadReceipt.CreateInstance(
                        ReadString(item, "Addressee") ?? string.Empty,
                        ReadLong(item, "Moment") ?? 0))
                    .ToList();
            }
        }

        var last = events.OrderBy(item => item.Version).Last();

        return new AnemicModel
        {
            Id = last.Id,
            Version = last.Version,
            CommandName = last.CommandName,
            SubjectName = last.SubjectName,
            CorrelationToken = last.CorrelationToken,
            Root = MessageRoot.CreateInstance(product, role, timestamp),
            Text = text.Length == 0 ? MessageText.Empty() : MessageText.CreateInstance(text),
            Addressee = MessageAddressee.CreateInstance(string.Join(" + ", to)),
            Attachments = MessageAttachments.CreateInstance(attachments),
            ReplyTo = replyTo,
            ExtendedFields = MessageExtendedFields.CreateInstance(extendedFields),
            ReadReceipts = receipts
        };
    }

    private static string? ReadString(JsonElement element, string property)
        => element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static long? ReadLong(JsonElement element, string property)
        => element.TryGetProperty(property, out var value) && value.TryGetInt64(out var number) ? number : null;
}
