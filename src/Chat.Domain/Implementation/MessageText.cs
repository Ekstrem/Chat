using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>
/// Текст сообщения. Инварианты: не пустой, ≤ 32 КБ, UTF-8 без BOM и без символов-заменителей.
/// </summary>
internal sealed class MessageText : IMessageText
{
    /// <summary>Максимальный размер текста в байтах UTF-8.</summary>
    public const int MaxBytes = 32 * 1024;

    public string Value { get; init; } = string.Empty;

    private MessageText() { }

    public static bool TryCreate(string? text, out IMessageText result, out string reason)
    {
        result = Empty();
        reason = string.Empty;

        if (string.IsNullOrWhiteSpace(text))
        {
            reason = "text: пустой текст сообщения";
            return false;
        }

        if (text.Length > 0 && text[0] == '\uFEFF')
        {
            reason = "text: BOM в начале текста запрещён (UTF-8 без BOM)";
            return false;
        }

        if (text.Contains('\uFFFD', StringComparison.Ordinal))
        {
            reason = "text: обнаружены символы-заменители (нарушена кодировка UTF-8)";
            return false;
        }

        if (System.Text.Encoding.UTF8.GetByteCount(text) > MaxBytes)
        {
            reason = $"text: превышен предел {MaxBytes} байт UTF-8";
            return false;
        }

        result = new MessageText { Value = text };
        return true;
    }

    public static IMessageText CreateInstance(string text)
        => TryCreate(text, out var result, out var reason)
            ? result
            : throw new ArgumentException(reason, nameof(text));

    public static IMessageText Empty() => new MessageText { Value = string.Empty };
}
