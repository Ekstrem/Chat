using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.InternalContracts;
using Newtonsoft.Json.Linq;
using System.Linq;
using Chat.Domain;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Monads;

namespace Chat.DomainServices
{
    //public class ChatDomainEventCommand : BusDomainEventCommand<IChat>
    //{
    //    public ChatDomainEventCommand(IDomainEvent<IChat> domainEvent): base(domainEvent)
    //    {
    //    }
    //}

    //public static class Extension
    //{
    //    public static IChatAnemicModel ToAnemicModel(this ChatDomainEventCommand notification)
    //    {
    //        var root = notification.ChangedValueObjects.TryGetValue(
    //                    nameof(IChatAnemicModel.Root), out var rawRoot)
    //                && rawRoot is ValueObject voRoot
    //            ? JObject.Parse(voRoot.Data.ToString())
    //                .PipeTo(json => ChatRoot
    //                    .CreateInstance(
    //                        (string)json["UserId"],
    //                        (int)json["SessionId"]))
    //            : null;

    //        var actor = notification.ChangedValueObjects.TryGetValue(
    //                    nameof(IChatAnemicModel.Actor), out var rawActor)
    //                && rawActor is ValueObject voActor
    //            ? JObject.Parse(voActor.Data.ToString())
    //                .PipeTo(json => ChatActor
    //                    .CreateInstance(
    //                        (string)json["Login"],
    //                        (UserType)json["Type"]))
    //            : null;

    //        var feedback = notification.ChangedValueObjects.TryGetValue(
    //                    nameof(IChatAnemicModel.Feedback), out var rawFeedback)
    //                && rawFeedback is ValueObject voFeedback
    //            ? JObject.Parse(voFeedback.Data.ToString())
    //                .PipeTo(json => ChatFeedback
    //                    // TODO: check arg
    //                    .CreateInstance(
    //                        (string)json["Text"]))
    //            : null;

    //        var messages = Enumerable.Empty<IChatMessage>();

    //        return AnemicModel.Create(
    //            notification.AggregateId,
    //            notification.Command,
    //            root,
    //            actor,
    //            feedback,
    //            messages);
    //    }
    //}
}
