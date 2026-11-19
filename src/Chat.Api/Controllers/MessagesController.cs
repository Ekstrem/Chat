using Chat.Application;
using Chat.Application.Commands;
using Chat.Application.Queries;
using Chat.InternalContracts.Views;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers;

/// <summary>Контракт канала: команды записи и запросы чтения (03-command-api, 04-query-api).</summary>
[ApiController]
[Route("messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Messages.SendMessage — создать сообщение.</summary>
    [HttpPost("send")]
    public async Task<ActionResult<ChatOperationResult>> Send([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new SendMessageCommand(
                request.CorrelationToken ?? Guid.NewGuid(),
                request.Product,
                request.Role,
                request.To,
                request.Text,
                request.Attachments,
                request.ReplyTo,
                request.ExtendedFields), cancellationToken)
            .ConfigureAwait(false);

        return ToActionResult(result);
    }

    /// <summary>Messages.AddAttachment — добавить вложение.</summary>
    [HttpPost("{id:guid}/attachments")]
    public async Task<ActionResult<ChatOperationResult>> AddAttachment(Guid id, [FromBody] AddAttachmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new AddAttachmentCommand(
                request.CorrelationToken ?? Guid.NewGuid(),
                id,
                request.AttachmentId,
                request.Role), cancellationToken)
            .ConfigureAwait(false);

        return ToActionResult(result);
    }

    /// <summary>Messages.ReadMessage — отметить сообщение прочитанным.</summary>
    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult<ChatOperationResult>> Read(Guid id, [FromBody] ReadMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new ReadMessageCommand(
                request.CorrelationToken ?? Guid.NewGuid(),
                id,
                request.Addressee), cancellationToken)
            .ConfigureAwait(false);

        return ToActionResult(result);
    }

    /// <summary>Messages.ReadInbox — только адресованное роли и новое (MCP read_inbox).</summary>
    [HttpGet("inbox")]
    public async Task<ActionResult<InboxView>> Inbox([FromQuery] string role, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ReadInboxQuery(role), cancellationToken).ConfigureAwait(false));

    /// <summary>Messages.Recent — сводка «что нового» после метки.</summary>
    [HttpGet("recent")]
    public async Task<ActionResult<SummaryView>> Recent([FromQuery] long since = 0, [FromQuery] int limit = 200, CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new RecentQuery(since, limit), cancellationToken).ConfigureAwait(false));

    /// <summary>Messages.Search — поиск по истории.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<MessageView>>> Search(
        [FromQuery] string? text = null,
        [FromQuery] string? to = null,
        [FromQuery] string? from = null,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new SearchQuery(text, to, from, limit), cancellationToken).ConfigureAwait(false));

    /// <summary>Messages.LastTimestamp — последняя метка чата.</summary>
    [HttpGet("last-timestamp")]
    public async Task<ActionResult<long>> LastTimestamp(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new LastTimestampQuery(), cancellationToken).ConfigureAwait(false));

    private ActionResult<ChatOperationResult> ToActionResult(ChatOperationResult result)
        => result.Success ? Ok(result) : BadRequest(result);
}

/// <summary>Тело запроса SendMessage.</summary>
public sealed class SendMessageRequest
{
    /// <summary>Маркер корреляции команды.</summary>
    public Guid? CorrelationToken { get; set; }

    /// <summary>Проект.</summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>Роль-автор.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Адресаты: @роль, @A + @B или ALL.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Текст сообщения.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Вложения.</summary>
    public Guid[]? Attachments { get; set; }

    /// <summary>Ответ на сообщение.</summary>
    public Guid? ReplyTo { get; set; }

    /// <summary>Расширяемые поля канала.</summary>
    public Dictionary<string, string>? ExtendedFields { get; set; }
}

/// <summary>Тело запроса AddAttachment.</summary>
public sealed class AddAttachmentRequest
{
    /// <summary>Маркер корреляции команды.</summary>
    public Guid? CorrelationToken { get; set; }

    /// <summary>Идентификатор вложения.</summary>
    public Guid AttachmentId { get; set; }

    /// <summary>Роль, добавляющая вложение.</summary>
    public string Role { get; set; } = string.Empty;
}

/// <summary>Тело запроса ReadMessage.</summary>
public sealed class ReadMessageRequest
{
    /// <summary>Маркер корреляции команды.</summary>
    public Guid? CorrelationToken { get; set; }

    /// <summary>Адресат, прочитавший сообщение.</summary>
    public string Addressee { get; set; } = string.Empty;
}
