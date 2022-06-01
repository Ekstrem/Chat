using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.InternalContracts;
using Newtonsoft.Json.Linq;
using System.Linq;
using Chat.Domain;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Monads;
using Hive.SeedWorks.TacticalPatterns;
using MHive.SeedWorks.Events.Converters;
using System;
using System.Collections.Generic;

namespace Chat.DomainServices
{
    public static class Extension
    {
        public static IChatAnemicModel ToAnemicModel(this ChatDomainEventCommand notification)
        {
            var root = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Root), out var rawRoot)
                    && rawRoot is StringValueObject voRoot
                ? JObject.Parse(voRoot.Data.ToString())
                    .PipeTo(json => ChatRoot
                        .CreateInstance(
                            (Guid)json["UserId"],
                            (int)json["SessionId"]))
                : null;

            var actor = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Actor), out var rawActor)
                    && rawActor is StringValueObject voActor
                ? JObject.Parse(voActor.Data.ToString())
                    .PipeTo(json => ChatActor
                        .CreateInstance(
                            (string)json["Login"],
                         (UserType)((int)json["Type"])))
                : null;

            var feedback = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Feedback), out var rawFeedback)
                    && rawFeedback is StringValueObject voFeedback
                ? JObject.Parse(voFeedback.Data.ToString())
                    .PipeTo(json => ChatFeedback
                        .CreateByText(
                            (string)json["Text"]))
                : null;

            var messages = notification.ChangedValueObjects.TryGetValue(
                        nameof(IChatAnemicModel.Messages), out var rawMessages)
                    && rawMessages is StringValueObject voMessages
              ? JObject.Parse(voMessages.Data.ToString())
                    .PipeTo(json =>
                        json["Value"]
                            .Select(x =>
                                ChatMessage.CreateInstance(
                                    (MessageType)((int)x["Type"]),
                                    (string)x["Text"],
                                    (Platform)((int)x["Platform"]),
                                    (Application)((int)x["Application"]),
                                    (Guid)x["ContentId"]
                                ))
                            .ToList<IChatMessage>())
              : new List<IChatMessage>();

            return AnemicModel.Create(
                notification.Id,
                notification.Command,
                root,
                actor,
                feedback,
                messages);
        }
    }
}
