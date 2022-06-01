using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Hive.SeedWorks.Monads;

namespace Chat.InternalContracts
{
    public static class Extension
    {
        //public static IChatAnemicModel ToAnemicModel(this DomainEventEntry domainEvent)
        //    => ToAnemicModel(domainEvent.ValueObjects, domainEvent.AggregateId, domainEvent.AggregateVersion,
        //          domainEvent.Command.CorrelationToken, domainEvent.Command.CommandName, domainEvent.Command.SubjectName);

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