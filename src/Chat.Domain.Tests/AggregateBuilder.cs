using System;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;

namespace Chat.Domain.Tests
{
    public static class AggregateBuilder
    {
        public static IChatAnemicModel CreatePureModelForNewChat(Guid id)
        {
            var userRequest = QuestionBuilder
                .DefaultUserRequest();

            return AnemicModel
                .Create(
                    id,
                    nameof(Aggregate.SubscriberRequestQuestion)
                        .CreateCommandMetadata(),
                    ChatRoot
                        .CreateInstance(
                            userRequest.UserId,
                            userRequest.SessionId),
                    ChatActor.CreateInstance(
                            userRequest.UserLogin,
                            (byte)UserType.Client),
                    default,
                    new[]
                    {
                        ChatMessage
                            .CreateInstance(
                                userRequest.Type,
                                userRequest.Text,
                                userRequest.Platform,
                                userRequest.Application,
                                userRequest.ContentId)
                    });
        }

        public static IChatAnemicModel CreateFeedbackModel(Guid id)
        {
            return AnemicModel
                .Create(
                    id,
                    nameof(Aggregate.SubscriberGaveFeedback)
                        .CreateCommandMetadata(),
                    default,
                    default,
                    ChatFeedback.CreateByText("нуштош сойдет"),
                    default);
        }

        public static IChatAnemicModel CreateBotReplyModel(Guid id)
        {
            return AnemicModel
                .Create(
                    id,
                    nameof(Aggregate.BotRepliedToUser)
                        .CreateCommandMetadata(),
                    default,
                    default,
                    default,
                    new[]
                    {
                        ChatMessage
                            .CreateInstance(
                                MessageType.Text,
                                "Автоматический ответ бота",
                                Platform.Windows,
                                Application.Site,
                                default)
                    });
        }

        public static IChatAnemicModel CreateOperatorReplyModel(Guid id)
        {
            return AnemicModel
                .Create(
                    id,
                    nameof(Aggregate.OperatorRepliedToMessage)
                        .CreateCommandMetadata(),
                    default,
                    default,
                    default,
                    new[]
                    {
                        ChatMessage
                            .CreateInstance(
                                MessageType.Text,
                                "Ответ оператора",
                                Platform.Windows,
                                Application.Site,
                                default)
                    });
        }

        public static IChatAnemicModel CreateFeedbackByScoresModel(Guid id)
        {
            return AnemicModel
                .Create(
                    id,
                    nameof(Aggregate.SubscriberGaveFeedback)
                        .CreateCommandMetadata(),
                    default,
                    default,
                    ChatFeedback.CreateByScores(5),
                    default);
        }
    }
}
