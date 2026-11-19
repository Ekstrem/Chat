namespace Chat.Application;

/// <summary>Результат команды канала: подтверждение отправки либо причина отказа.</summary>
public sealed class ChatOperationResult
{
    private ChatOperationResult(bool success, Guid messageId, IReadOnlyList<string> reasons)
    {
        Success = success;
        MessageId = messageId;
        Reasons = reasons;
    }

    /// <summary>Операция выполнена.</summary>
    public bool Success { get; }

    /// <summary>Идентификатор сообщения (при успехе).</summary>
    public Guid MessageId { get; }

    /// <summary>Нарушенные инварианты (при отказе).</summary>
    public IReadOnlyList<string> Reasons { get; }

    /// <summary>Успешный результат.</summary>
    public static ChatOperationResult Ok(Guid messageId) => new(true, messageId, Array.Empty<string>());

    /// <summary>Отказ с причинами.</summary>
    public static ChatOperationResult Fail(IEnumerable<string> reasons)
        => new(false, Guid.Empty, reasons.Where(reason => !string.IsNullOrWhiteSpace(reason)).ToList());

    /// <summary>Отказ с одной причиной.</summary>
    public static ChatOperationResult Fail(string reason) => Fail(new[] { reason });
}
