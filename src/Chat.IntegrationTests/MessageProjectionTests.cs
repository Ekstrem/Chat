using Chat.Domain.Abstraction;
using Chat.IntegrationTests;
using DigiTFactory.Libraries.SeedWorks.Events;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chat.IntegrationTests;

/// <summary>
/// Level 2 (РєРѕРјРїРѕРЅРµРЅС‚РЅС‹Рµ): СЃРѕР±С‹С‚РёСЏ Р°РіСЂРµРіР°С‚Р° РїСЂРѕРµС†РёСЂСѓСЋС‚СЃСЏ РІ MV messages/inbox/last_read.
/// </summary>
public sealed class MessageProjectionTests
{
    [Fact]
    public async Task SendMessage_projects_message_and_inbox_rows()
    {
        using var harness = new ProjectionHarness();
        var aggregateId = Guid.NewGuid();
        var aggregate = ProjectionHarness.EmptyAggregate(aggregateId);

        var result = aggregate.SendMessage(
            CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE"),
            "Chat",
            "@BE",
            "@DO",
            "РїСЂРёРІРµС‚, РјРёСЂ",
            null,
            null,
            null);

        await harness.Handler.ProjectAsync(result.Event);

        var message = await harness.ReadDb.Messages.SingleAsync(row => row.Id == aggregateId);
        Assert.Equal("РїСЂРёРІРµС‚, РјРёСЂ", message.Text);
        Assert.Equal("@BE", message.Role);
        Assert.Equal("Chat", message.Product);
        Assert.Equal(new[] { "@DO" }, message.To);

        var inbox = await harness.ReadDb.Inbox.SingleAsync(row => row.MessageId == aggregateId);
        Assert.Equal("@DO", inbox.Role);
        Assert.Equal("РїСЂРёРІРµС‚, РјРёСЂ", inbox.Text);
        Assert.Null(inbox.ReadAt);
    }

    [Fact]
    public async Task SendMessage_to_multiple_addressees_creates_row_per_addressee()
    {
        using var harness = new ProjectionHarness();
        var aggregateId = Guid.NewGuid();

        var result = ProjectionHarness.EmptyAggregate(aggregateId).SendMessage(
            CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE"),
            "Chat",
            "@BE",
            "@DO + @TE",
            "РґРІРѕРёРј",
            null,
            null,
            null);

        await harness.Handler.ProjectAsync(result.Event);

        var roles = await harness.ReadDb.Inbox
            .Where(row => row.MessageId == aggregateId)
            .Select(row => row.Role)
            .OrderBy(role => role)
            .ToListAsync();

        Assert.Equal(new[] { "@DO", "@TE" }, roles);
    }

    [Fact]
    public async Task ReadMessage_updates_inbox_read_at_and_cursor()
    {
        using var harness = new ProjectionHarness();
        var aggregateId = Guid.NewGuid();
        var send = ProjectionHarness.EmptyAggregate(aggregateId).SendMessage(
            CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE"),
            "Chat",
            "@BE",
            "@DO",
            "РїСЂРѕС‡РёС‚Р°Р№ РјРµРЅСЏ",
            null,
            null,
            null);
        await harness.Handler.ProjectAsync(send.Event);

        var model = await harness.StateRepository.GetById(aggregateId, CancellationToken.None);
        var readCommand = CommandToAggregate.Commit(Guid.NewGuid(), "ReadMessage", "@DO");
        var read = Domain.Implementation.Aggregate.CreateInstance(model[0]).ReadMessage(readCommand, "@DO");
        await harness.Handler.ProjectAsync(read.Event);

        var inbox = await harness.ReadDb.Inbox.SingleAsync(row => row.MessageId == aggregateId);
        Assert.Equal(readCommand.Version, inbox.ReadAt);

        var cursor = await harness.ReadDb.LastRead.SingleAsync(row => row.Role == "@DO");
        Assert.Equal(readCommand.Version, cursor.LastReadAt);
    }

    [Fact]
    public async Task Repeated_projection_of_same_event_is_idempotent()
    {
        using var harness = new ProjectionHarness();
        var aggregateId = Guid.NewGuid();

        var result = ProjectionHarness.EmptyAggregate(aggregateId).SendMessage(
            CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE"),
            "Chat",
            "@BE",
            "ALL",
            "С€РёСЂРѕРєРѕРІРµС‰Р°С‚РµР»СЊРЅРѕРµ",
            null,
            null,
            null);

        await harness.Handler.ProjectAsync(result.Event);
        await harness.Handler.ProjectAsync(result.Event);

        Assert.Equal(1, await harness.ReadDb.Messages.CountAsync(row => row.Id == aggregateId));
        Assert.Equal(1, await harness.ReadDb.Inbox.CountAsync(row => row.MessageId == aggregateId));
    }

    [Fact]
    public async Task Aggregate_state_is_restored_from_projection()
    {
        using var harness = new ProjectionHarness();
        var aggregateId = Guid.NewGuid();

        var result = ProjectionHarness.EmptyAggregate(aggregateId).SendMessage(
            CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE"),
            "Chat",
            "@BE",
            "@DO",
            "СЃРѕСЃС‚РѕСЏРЅРёРµ",
            null,
            null,
            null);
        await harness.Handler.ProjectAsync(result.Event);

        var model = await harness.StateRepository.GetById(aggregateId, CancellationToken.None);

        var restored = Assert.Single(model);
        Assert.Equal(aggregateId, restored.Id);
        Assert.Equal("СЃРѕСЃС‚РѕСЏРЅРёРµ", restored.Text.Value);
        Assert.Equal("@BE", restored.Root.Role);
    }

    [Fact]
    public async Task ReadDb_smoke_stores_and_loads_message_row()
    {
        using var harness = new ProjectionHarness();
        var id = Guid.NewGuid();

        harness.ReadDb.Messages.Add(new Chat.InternalContracts.MessageReadModel
        {
            Id = id,
            Product = "Chat",
            Role = "@BE",
            Timestamp = 1,
            Text = "текст",
            Version = 1
        });

        await harness.ReadDb.SaveChangesAsync();

        var loaded = await harness.ReadDb.Messages.FindAsync(id);
        Assert.NotNull(loaded);
    }}
