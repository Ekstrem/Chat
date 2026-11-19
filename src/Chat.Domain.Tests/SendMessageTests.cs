using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Result;
using Xunit;

namespace Chat.Domain.Tests;

/// <summary>Инварианты команды SendMessage (F-1, F-2, N-3).</summary>
public sealed class SendMessageTests
{
    [Fact]
    public void Send_creates_message_and_returns_success_event()
    {
        var aggregate = MessageFixture.EmptyAggregate();

        var result = aggregate.SendMessage(
            MessageFixture.Command("SendMessage"),
            MessageFixture.Product,
            MessageFixture.Role,
            "@DO",
            "текст сообщения",
            null,
            null,
            null);

        Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        Assert.NotEqual(Guid.Empty, result.Event.Id);
        Assert.Equal("SendMessage", result.Event.Command.CommandName);
    }

    [Fact]
    public void Send_rejects_empty_text()
    {
        var result = MessageFixture.EmptyAggregate().SendMessage(
            MessageFixture.Command("SendMessage"),
            MessageFixture.Product,
            MessageFixture.Role,
            "@DO",
            "   ",
            null,
            null,
            null);

        Assert.Equal(DomainOperationResultEnum.Exception, result.Result);
        Assert.Contains(result.Reason, reason => reason.Contains("text", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("@DO")]
    [InlineData("@DO + @BE")]
    [InlineData("ALL")]
    public void Send_accepts_valid_addressees(string to)
    {
        var result = MessageFixture.EmptyAggregate().SendMessage(
            MessageFixture.Command("SendMessage"),
            MessageFixture.Product,
            MessageFixture.Role,
            to,
            "текст",
            null,
            null,
            null);

        Assert.Equal(DomainOperationResultEnum.Success, result.Result);
    }

    [Theory]
    [InlineData("DO")]
    [InlineData("@")]
    [InlineData("")]
    public void Send_rejects_invalid_addressee(string to)
    {
        var result = MessageFixture.EmptyAggregate().SendMessage(
            MessageFixture.Command("SendMessage"),
            MessageFixture.Product,
            MessageFixture.Role,
            to,
            "текст",
            null,
            null,
            null);

        Assert.Equal(DomainOperationResultEnum.Exception, result.Result);
    }

    [Fact]
    public void Send_rejects_text_with_bom()
    {
        var result = MessageFixture.EmptyAggregate().SendMessage(
            MessageFixture.Command("SendMessage"),
            MessageFixture.Product,
            MessageFixture.Role,
            "@DO",
            "\uFEFFтекст",
            null,
            null,
            null);

        Assert.Equal(DomainOperationResultEnum.Exception, result.Result);
    }

    [Fact]
    public void Send_rejects_text_with_replacement_characters()
    {
        var result = MessageFixture.EmptyAggregate().SendMessage(
            MessageFixture.Command("SendMessage"),
            MessageFixture.Product,
            MessageFixture.Role,
            "@DO",
            "текст \uFFFD",
            null,
            null,
            null);

        Assert.Equal(DomainOperationResultEnum.Exception, result.Result);
    }

    [Fact]
    public void Send_uses_command_version_as_timestamp()
    {
        var version = 1_700_000_000_123L;

        var result = MessageFixture.EmptyAggregate().SendMessage(
            MessageFixture.Command("SendMessage", version),
            MessageFixture.Product,
            MessageFixture.Role,
            "@DO",
            "текст",
            null,
            null,
            null);

        var model = (IChatAnemicModel)result.BusinessOperationData.Model;
        Assert.Equal(version, model.Root.Timestamp);
        Assert.Equal(version, model.Version);
    }
}
