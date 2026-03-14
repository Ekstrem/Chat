using System;
using System.Linq;
using System.Text.Json;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.InternalContracts;
using DigiTFactory.Libraries.SeedWorks.Monads;

namespace Chat.DomainServices
{
    public static class Extension
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Восстанавливает анемичную модель из команды доменного события.
        /// </summary>
        public static IChatAnemicModel ToAnemicModel(this ChatDomainEventCommand notification)
        {
            var root = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Root), out var rawRoot)
                    && rawRoot != null
                ? JsonSerializer.Serialize(rawRoot)
                    .PipeTo(json => JsonDocument.Parse(json).RootElement)
                    .PipeTo(el => ChatRoot
                        .CreateInstance(
                            el.GetProperty("UserId").GetString()!
                                .PipeTo(Guid.Parse),
                            el.GetProperty("SessionId").GetInt32()))
                : null;

            var actor = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Actor), out var rawActor)
                    && rawActor != null
                ? JsonSerializer.Serialize(rawActor)
                    .PipeTo(json => JsonDocument.Parse(json).RootElement)
                    .PipeTo(el => ChatActor
                        .CreateInstance(
                            el.GetProperty("Login").GetString()!,
                            el.GetProperty("Type").GetInt32()
                                .PipeTo(t => (UserType)t)))
                : null;

            var feedback = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Feedback), out var rawFeedback)
                    && rawFeedback != null
                ? JsonSerializer.Serialize(rawFeedback)
                    .PipeTo(json => JsonDocument.Parse(json).RootElement)
                    .PipeTo(el => el.GetProperty("Type").GetInt32() == (int)AnswerType.Scores
                        ? ChatFeedback.CreateByScores(el.GetProperty("Value").GetByte())
                        : ChatFeedback.CreateByText(el.GetProperty("Text").GetString()!))
                : null;

            var messages = Enumerable.Empty<IChatMessage>();

            return AnemicModel.Create(
                notification.AggregateId,
                notification.Command,
                root,
                actor,
                feedback,
                messages);
        }
    }
}
