namespace Chat.InternalContracts.Views;

/// <summary>Элемент inbox: только адресованное роли и новее её курсора.</summary>
public sealed class InboxItemView
{
    /// <summary>Идентификатор сообщения.</summary>
    public Guid MessageId { get; set; }

    /// <summary>Автор.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Метка времени, Unix ms.</summary>
    public long Timestamp { get; set; }

    /// <summary>Текст.</summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>Ответ <c>Messages.ReadInbox</c> (MCP <c>read_inbox</c>).</summary>
public sealed class InboxView
{
    /// <summary>Роль, для которой построен inbox.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Курсор, с которого читали.</summary>
    public long? Since { get; set; }

    /// <summary>Новые сообщения по возрастанию метки.</summary>
    public List<InboxItemView> Messages { get; set; } = new();
}

/// <summary>Элемент сводки «что нового» (полный контекст для субагента).</summary>
public sealed class SummaryItemView
{
    /// <summary>Идентификатор сообщения.</summary>
    public Guid MessageId { get; set; }

    /// <summary>Автор.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Адресаты.</summary>
    public List<string> To { get; set; } = new();

    /// <summary>Метка времени, Unix ms.</summary>
    public long Timestamp { get; set; }

    /// <summary>Текст.</summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>Ответ <c>Messages.Recent</c>.</summary>
public sealed class SummaryView
{
    /// <summary>Метка, после которой собрана сводка.</summary>
    public long Since { get; set; }

    /// <summary>Сообщения по возрастанию метки.</summary>
    public List<SummaryItemView> Messages { get; set; } = new();
}

/// <summary>Полная проекция сообщения для поиска и резолва идентификаторов.</summary>
public sealed class MessageView
{
    /// <summary>Идентификатор сообщения.</summary>
    public Guid Id { get; set; }

    /// <summary>Проект.</summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>Автор.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Метка времени, Unix ms.</summary>
    public long Timestamp { get; set; }

    /// <summary>Адресаты.</summary>
    public List<string> To { get; set; } = new();

    /// <summary>Текст.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Вложения.</summary>
    public List<Guid> Attachments { get; set; } = new();

    /// <summary>Ответ на сообщение.</summary>
    public Guid? ReplyTo { get; set; }

    /// <summary>Расширяемые поля.</summary>
    public Dictionary<string, string> ExtendedFields { get; set; } = new();

    /// <summary>Отметки о прочтении.</summary>
    public List<ReadReceiptItemView> ReadReceipts { get; set; } = new();
}

/// <summary>Отметка о прочтении в ответе API.</summary>
public sealed class ReadReceiptItemView
{
    /// <summary>Адресат.</summary>
    public string Addressee { get; set; } = string.Empty;

    /// <summary>Момент прочтения, Unix ms.</summary>
    public long Moment { get; set; }
}
