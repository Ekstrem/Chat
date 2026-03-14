using System;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Monads;
using DigiTFactory.Libraries.SeedWorks.Result;
using Xunit;

namespace Chat.Domain.Tests
{
    public class AggregateTest
    {
        [Fact]
        public void SubscriberRequestQuestion()
        {
            var aggregateId = Guid.NewGuid();
            var pureModel = AggregateBuilder.CreatePureModelForNewChat(aggregateId);

            var result = DefaultAnemicModel
                .Create(pureModel.Id)
                .PipeTo(Aggregate.Create)
                .SubscriberRequestQuestion(pureModel);

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void SubscriberGaveFeedback()
        {
            var aggregateId = Guid.NewGuid();
            var feedbackModel = AggregateBuilder.CreateFeedbackModel(aggregateId);

            var result = DefaultAnemicModel
                .Create(feedbackModel.Id)
                .PipeTo(Aggregate.Create)
                .SubscriberGaveFeedback(feedbackModel, feedbackModel.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void BotRepliedToUser_Success()
        {
            var aggregateId = Guid.NewGuid();
            var pureModel = AggregateBuilder.CreatePureModelForNewChat(aggregateId);
            var existingAggregate = Aggregate.Create(pureModel);
            var model = AggregateBuilder.CreateBotReplyModel(aggregateId);

            var result = existingAggregate.BotRepliedToUser(model, model.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void OperatorRepliedToMessage_Success()
        {
            var aggregateId = Guid.NewGuid();
            var pureModel = AggregateBuilder.CreatePureModelForNewChat(aggregateId);
            var existingAggregate = Aggregate.Create(pureModel);
            var model = AggregateBuilder.CreateOperatorReplyModel(aggregateId);

            var result = existingAggregate.OperatorRepliedToMessage(model, model.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void SubscriberGaveFeedback_ByScores()
        {
            var aggregateId = Guid.NewGuid();
            var model = AggregateBuilder.CreateFeedbackByScoresModel(aggregateId);
            var existingAggregate = DefaultAnemicModel.Create(aggregateId).PipeTo(Aggregate.Create);

            var result = existingAggregate.SubscriberGaveFeedback(model, model.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void OperatorDequeueRequest_ThrowsNotImplemented()
        {
            var aggregateId = Guid.NewGuid();
            var model = AggregateBuilder.CreateBotReplyModel(aggregateId);
            var existingAggregate = DefaultAnemicModel.Create(aggregateId).PipeTo(Aggregate.Create);

            Assert.Throws<NotImplementedException>(() =>
                existingAggregate.OperatorDequeueRequest(model, model.CommandName.CreateCommandMetadata()));
        }

        [Fact]
        public void SessionEndingByTrigger_ThrowsNotImplemented()
        {
            var aggregateId = Guid.NewGuid();
            var model = AggregateBuilder.CreateBotReplyModel(aggregateId);
            var existingAggregate = DefaultAnemicModel.Create(aggregateId).PipeTo(Aggregate.Create);

            Assert.Throws<NotImplementedException>(() =>
                existingAggregate.SessionEndingByTrigger(model, model.CommandName.CreateCommandMetadata()));
        }
    }
}
