using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Hive.SeedWorks.Monads;
using MHive.SeedWorks.Events.Converters;
using System.Linq;

namespace Chat.InternalContracts
{
    public static class Extension
    {
        //public static IChatAnemicModel ToAnemicModel(this DomainEventEntry domainEvent)
        //    => ToAnemicModel(domainEvent.ValueObjects, domainEvent.AggregateId, domainEvent.AggregateVersion,
        //          domainEvent.Command.CorrelationToken, domainEvent.Command.CommandName, domainEvent.Command.SubjectName);

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

        public static IChatAnemicModel ToAnemicModel(this string valueObjects, Guid id, long version, Guid correlationToken, string commandName, string subjectName)
        {
            var json = JObject.Parse(valueObjects);

            var root = nameof(IChatAnemicModel.Root)
                .Either(
                    c => json.ContainsKey(c),
                    voName => ChatRoot.CreateInstance(
                        (Guid)json[voName]["UserId"],
                        (int)json[voName]["SessionId"]),
                    f => null);

            var actor = nameof(IChatAnemicModel.Actor)
                .Either(
                    c => json.ContainsKey(c),
                    voName => ChatActor.CreateInstance(
                        (string)json[voName]["Login"],
                        (UserType)((int)json[voName]["Type"])),
                    f => null);


            var feedback = nameof(IChatAnemicModel.Feedback)
                .Either(
                    c => json.ContainsKey(c),
                    voName => ChatFeedback.CreateByText(
                        (string)json[voName]["Text"]),
                    f => null);

            List<IChatMessage> messages = new List<IChatMessage>();

            //var message2s = nameof(IChatAnemicModel.Messages)
            //    .Either(
            //        c => json.ContainsKey(c),
            //        voName =>
            //        {
            //            var vo = json[voName];
            //            var fff = vo["Messages"].ToObject<IEnumerable<ChatMessage>>();

            //            //ChatMessage.CreateInstance(
            //            //(string)json[voName]["Login"],
            //            //UserType.Client),

            //            return Enumerable.Empty<IChatMessage>();

            //        },
            //        f => Enumerable.Empty<IChatMessage>());

            return AnemicModel.Create(id, version, correlationToken, commandName, subjectName, root, actor, feedback, messages);
        }
    }
}