using System;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;

namespace Chat.Domain.Tests
{
    public static class OperatorBuilder
    {
        public static IChatAnemicModel CreateOperatorDequeueModel(Guid id)
        {
            return AnemicModel
                .Create(
                    id,
                    nameof(Aggregate.OperatorDequeueRequest)
                        .CreateCommandMetadata(),
                    default,
                    ChatActor.CreateInstance("Оператор Петров", UserType.Operator),
                    default,
                    new[]
                    {
                        ChatMessage.CreateInstance(
                            MessageType.Text,
                            "Оператор взял обращение в работу",
                            Platform.Windows,
                            Application.Site,
                            Guid.Empty)
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
                        ChatMessage.CreateInstance(
                            MessageType.Text,
                            "Ответ оператора на обращение",
                            Platform.Windows,
                            Application.Site,
                            Guid.Empty)
                    });
        }
    }
}
