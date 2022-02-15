using AutoMapper;
using Chat.Application.Commands;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Events;
using System;

namespace Chat.Application.Mappers
{
    public class AggregateProfile : Profile
    {
        public AggregateProfile()
        {
            CreateMap<RequestQuestionCommand, IChatAnemicModel>();
               
                //.ConstructUsing(x => 
                //{
                //    return AnemicModel
                //        .Create(
                //           Guid.NewGuid(),
                //            CreateCommandMetadata(
                //                nameof(Aggregate.SubscriberRequestQuestion), 
                //                x.CorrelationId),
                //            ChatRoot.CreateInstance(
                //                Guid.NewGuid(),
                //                23453254),
                //            ChatActor.CreateInstance(
                //                x.UserName,
                //                (byte)UserType.Client),
                //            default,
                //            new[]
                //            {
                //                ChatMessage
                //                    .CreateInstance(
                //                        x.Type,
                //                        x.Message,
                //                        Platform.Android,
                //                        x.Application,
                //                        Guid.Empty)
                //            });
                //});
        }

        public ICommandToAggregate CreateCommandMetadata(string commandName, Guid correlationToken, long version = 0)
            => CommandToAggregate.Commit(
                correlationToken,
                commandName,
                nameof(IChat),
                version);
    }
}
