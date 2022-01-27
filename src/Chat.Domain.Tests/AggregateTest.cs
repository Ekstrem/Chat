using System;
using Chat.Domain.Implementation;
using Hive.SeedWorks.Monads;
using Hive.SeedWorks.Result;
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
    }
}
