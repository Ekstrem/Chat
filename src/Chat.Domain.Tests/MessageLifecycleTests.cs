using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Result;
using Xunit;

namespace Chat.Domain.Tests;

/// <summary>Инварианты AddAttachment и ReadMessage.</summary>
public sealed class MessageLifecycleTests
{
    [Fact]
    public void AddAttachment_adds_attachment_to_existing_message()
    {
        var aggregate = MessageFixture.ExistingMessage();
        var attachmentId = Guid.NewGuid();

        var result = aggregate.AddAttachment(MessageFixture.Command("AddAttachment", 2), attachmentId);

        Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        var model = (IChatAnemicModel)result.BusinessOperationData.Model;
        Assert.Contains(attachmentId, model.Attachments.Items);
    }

    [Fact]
    public void AddAttachment_does_not_duplicate_same_attachment()
    {
        var aggregate = MessageFixture.ExistingMessage();
        var attachmentId = Guid.NewGuid();

        aggregate.AddAttachment(MessageFixture.Command("AddAttachment", 2), attachmentId);
        var result = aggregate.AddAttachment(MessageFixture.Command("AddAttachment", 3), attachmentId);

        var model = (IChatAnemicModel)result.BusinessOperationData.Model;
        Assert.Single(model.Attachments.Items);
    }

    [Fact]
    public void AddAttachment_rejects_unknown_message()
    {
        var result = MessageFixture.EmptyAggregate()
            .AddAttachment(MessageFixture.Command("AddAttachment"), Guid.NewGuid());

        Assert.Equal(DomainOperationResultEnum.Exception, result.Result);
        Assert.Contains(result.Reason, reason => reason.Contains("не найдено", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadMessage_adds_receipt_for_addressee()
    {
        var result = MessageFixture.ExistingMessage()
            .ReadMessage(MessageFixture.Command("ReadMessage", 5), "@DO");

        Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        var model = (IChatAnemicModel)result.BusinessOperationData.Model;
        var receipt = Assert.Single(model.ReadReceipts);
        Assert.Equal("@DO", receipt.Addressee);
        Assert.Equal(5, receipt.Moment);
    }

    [Fact]
    public void ReadMessage_replaces_receipt_of_same_addressee()
    {
        var aggregate = MessageFixture.ExistingMessage();
        aggregate.ReadMessage(MessageFixture.Command("ReadMessage", 5), "@DO");

        var result = aggregate.ReadMessage(MessageFixture.Command("ReadMessage", 9), "@DO");

        var model = (IChatAnemicModel)result.BusinessOperationData.Model;
        Assert.Single(model.ReadReceipts);
        Assert.Equal(9, model.ReadReceipts[0].Moment);
    }

    [Fact]
    public void ReadMessage_rejects_empty_addressee()
    {
        var result = MessageFixture.ExistingMessage()
            .ReadMessage(MessageFixture.Command("ReadMessage", 5), "  ");

        Assert.Equal(DomainOperationResultEnum.Exception, result.Result);
    }
}
