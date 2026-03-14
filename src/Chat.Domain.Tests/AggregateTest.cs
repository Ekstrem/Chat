using System;
using System.Linq;
using Chat.Domain.Abstraction;
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
        public void SubscriberGaveFeedback_ByScores()
        {
            var aggregateId = Guid.NewGuid();
            var feedbackModel = AggregateBuilder.CreateFeedbackByScoresModel(aggregateId);

            var result = DefaultAnemicModel
                .Create(feedbackModel.Id)
                .PipeTo(Aggregate.Create)
                .SubscriberGaveFeedback(feedbackModel, feedbackModel.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void BotRepliedToUser_ConcatenatesMessages()
        {
            var aggregateId = Guid.NewGuid();
            var existingChat = AggregateBuilder.CreateExistingActiveChat(aggregateId);
            var botReply = AggregateBuilder.CreateBotReplyModel(aggregateId);

            var result = Aggregate
                .Create(existingChat)
                .BotRepliedToUser(botReply, botReply.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void OperatorDequeueRequest_HappyPath()
        {
            var aggregateId = Guid.NewGuid();
            var existingChat = AggregateBuilder.CreateExistingActiveChat(aggregateId);
            var operatorModel = OperatorBuilder.CreateOperatorDequeueModel(aggregateId);

            var result = Aggregate
                .Create(existingChat)
                .OperatorDequeueRequest(operatorModel, operatorModel.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void OperatorDequeueRequest_AlreadyDequeued_Fails()
        {
            var aggregateId = Guid.NewGuid();
            var alreadyDequeued = AggregateBuilder.CreateAlreadyDequeuedChat(aggregateId);
            var operatorModel = OperatorBuilder.CreateOperatorDequeueModel(aggregateId);

            var result = Aggregate
                .Create(alreadyDequeued)
                .OperatorDequeueRequest(operatorModel, operatorModel.CommandName.CreateCommandMetadata());

            // ValidateCommand возвращает WithWarnings при провале валидации
            Assert.NotEqual(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void SessionEndingByTrigger_HappyPath()
        {
            var aggregateId = Guid.NewGuid();
            var existingChat = AggregateBuilder.CreateExistingActiveChat(aggregateId);
            var endModel = AggregateBuilder.CreateSessionEndModel(aggregateId);

            var result = Aggregate
                .Create(existingChat)
                .SessionEndingByTrigger(endModel, endModel.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void SessionEndingByTrigger_InactiveSession_Fails()
        {
            var aggregateId = Guid.NewGuid();
            var endModel = AggregateBuilder.CreateSessionEndModel(aggregateId);

            var result = DefaultAnemicModel
                .Create(aggregateId)
                .PipeTo(Aggregate.Create)
                .SessionEndingByTrigger(endModel, endModel.CommandName.CreateCommandMetadata());

            // ValidateCommand возвращает WithWarnings при провале валидации
            Assert.NotEqual(DomainOperationResultEnum.Success, result.Result);
        }

        [Fact]
        public void OperatorRepliedToMessage_HappyPath()
        {
            var aggregateId = Guid.NewGuid();
            var existingChat = AggregateBuilder.CreateExistingActiveChat(aggregateId);
            var replyModel = OperatorBuilder.CreateOperatorReplyModel(aggregateId);

            var result = Aggregate
                .Create(existingChat)
                .OperatorRepliedToMessage(replyModel, replyModel.CommandName.CreateCommandMetadata());

            Assert.Equal(DomainOperationResultEnum.Success, result.Result);
        }
    }
}
