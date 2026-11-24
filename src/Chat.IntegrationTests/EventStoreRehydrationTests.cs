using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.IntegrationTests;
using Chat.Storage;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using Xunit;

namespace Chat.IntegrationTests;

/// <summary>
/// Level 2: состояние агрегата восстанавливается из журнала событий
/// через доменный IEventStreamRehydrator библиотечного Full Event Sourcing.
/// </summary>
public sealed class EventStoreRehydrationTests
{
    [Fact]
    public void SendMessage_event_restores_full_state()
    {
        var aggregateId = Guid.NewGuid();
        var command = CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE");

        var result = ProjectionHarness.EmptyAggregate(aggregateId).SendMessage(
            command,
            "Chat",
            "@BE",
            "@DO",
            "текст из события",
            new[] { Guid.Parse("11111111-1111-1111-1111-111111111111") },
            null,
            new Dictionary<string, string> { ["priority"] = "high" });

        var entry = DomainEventEntryFactory.Create(result);
        var restored = Assert.Single(new MessageEventStreamRehydrator().Rehydrate(new[] { entry }));

        Assert.Equal(aggregateId, restored.Id);
        Assert.Equal("Chat", restored.Root.Product);
        Assert.Equal("@BE", restored.Root.Role);
        Assert.Equal(command.Version, restored.Root.Timestamp);
        Assert.Equal("текст из события", restored.Text.Value);
        Assert.Equal(new[] { "@DO" }, restored.Addressee.To);
        Assert.Single(restored.Attachments.Items);
        Assert.Equal("high", restored.ExtendedFields.Items["priority"]);
        Assert.Equal(command.Version, restored.Version);
        Assert.Equal("SendMessage", restored.CommandName);
    }

    [Fact]
    public void Events_are_folded_in_version_order()
    {
        var aggregateId = Guid.NewGuid();
        var send = ProjectionHarness.EmptyAggregate(aggregateId).SendMessage(
            CommandToAggregate.Commit(Guid.NewGuid(), "SendMessage", "@BE", 1000),
            "Chat",
            "@BE",
            "@DO",
            "исходное",
            null,
            null,
            null);

        var attachmentId = Guid.NewGuid();
        var addAttachment = ProjectionHarness
            .AggregateOf((IChatAnemicModel)send.BusinessOperationData.Model)
            .AddAttachment(CommandToAggregate.Commit(Guid.NewGuid(), "AddAttachment", "@BE", 2000), attachmentId);

        var read = ProjectionHarness
            .AggregateOf((IChatAnemicModel)addAttachment.BusinessOperationData.Model)
            .ReadMessage(CommandToAggregate.Commit(Guid.NewGuid(), "ReadMessage", "@DO", 3000), "@DO");

        var entries = new[]
        {
            DomainEventEntryFactory.Create(send),
            DomainEventEntryFactory.Create(addAttachment),
            DomainEventEntryFactory.Create(read)
        };

        var restored = Assert.Single(new MessageEventStreamRehydrator().Rehydrate(entries));

        Assert.Equal("исходное", restored.Text.Value);
        Assert.Equal(new[] { attachmentId }, restored.Attachments.Items);
        var receipt = Assert.Single(restored.ReadReceipts);
        Assert.Equal("@DO", receipt.Addressee);
        Assert.Equal(3000, receipt.Moment);
        Assert.Equal(3000, restored.Version);
    }

    [Fact]
    public void Unknown_aggregate_yields_empty_event_list()
    {
        var aggregateId = Guid.NewGuid();
        var entries = Array.Empty<DomainEventEntry>();

        Assert.Empty(entries);
        Assert.NotEqual(Guid.Empty, aggregateId);
    }
}
