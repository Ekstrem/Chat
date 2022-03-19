//using Chat.Domain;
//using Chat.Domain.Abstraction;
//using Chat.Domain.Implementation;
//using Hive.SeedWorks.Monads;
//using Hive.SeedWorks.TacticalPatterns;
//using System.Collections.Generic;
//using System.Linq;

//namespace Chat.DomainServices
//{
//    internal class DomainCommandExecutor :
//        DomainCommandExecutor<IChat, IChatAggregate, IChatAnemicModel>
//    {
//        public DomainCommandExecutor(
//            IChatAggregateProvider provider) : base(provider)
//        { }

//        protected override IChatAnemicModel ToAnemicModel(IDictionary<string, IValueObject> valueObjects)
//            => valueObjects.PipeTo(vos => AnemicModel.Create(
//                AggregateId,
//                AggregateVersion,
//                CommandMetadata.CorrlationToken,
//                CommandMetadata.CommandName,
//                CommandMetadata.SubjectName,
//                vos.TryGetValue(nameof(ChatRoot), out var root) ? (ChatRoot)root : null,
//                vos.TryGetValue(nameof(ChatActor), out var actor) ? (ChatActor)actor : null,
//                vos.TryGetValue(nameof(ChatFeedback), out var feedback) ? (ChatFeedback)feedback : null,
//                Enumerable.Empty<ChatMessage>()));
//    }
//}
