using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation;

/// <summary>
/// Адресаты сообщения: один (<c>@BE</c>), несколько (<c>@BE + @DO</c>) или широковещательный (<c>ALL</c>).
/// </summary>
internal sealed class MessageAddressee : IAddressee
{
    public IReadOnlyList<string> To { get; init; } = Array.Empty<string>();

    private MessageAddressee() { }

    public static bool TryCreate(string? to, out IAddressee result, out string reason)
    {
        result = Empty();
        reason = string.Empty;

        if (string.IsNullOrWhiteSpace(to))
        {
            reason = "to: адресат обязателен";
            return false;
        }

        var tokens = to
            .Split('+', StringSplitOptions.RemoveEmptyEntries)
            .Select(token => token.Trim())
            .Where(token => token.Length > 0)
            .ToList();

        if (tokens.Count == 0)
        {
            reason = "to: адресат обязателен";
            return false;
        }

        foreach (var token in tokens)
        {
            if (string.Equals(token, "ALL", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (token.Length < 2 || token[0] != '@')
            {
                reason = $"to: невалидный адресат '{token}' — ожидается @роль или ALL";
                return false;
            }
        }

        var normalized = tokens
            .Select(token => string.Equals(token, "ALL", StringComparison.OrdinalIgnoreCase) ? "ALL" : token)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        result = new MessageAddressee { To = normalized };
        return true;
    }

    public static IAddressee CreateInstance(string to)
        => TryCreate(to, out var result, out var reason)
            ? result
            : throw new ArgumentException(reason, nameof(to));

    public static IAddressee Empty() => new MessageAddressee { To = Array.Empty<string>() };

    /// <summary>Признак широковещательного сообщения.</summary>
    public static bool IsBroadcast(IAddressee addressee)
        => addressee.To.Any(token => string.Equals(token, "ALL", StringComparison.Ordinal));
}
