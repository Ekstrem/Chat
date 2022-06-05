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
    internal class AggregateProfile : Profile
    {
        public AggregateProfile()
        {
            CreateMap<RequestQuestionCommand, IChatAnemicModel>();
        }

        public ICommandToAggregate CreateCommandMetadata(string commandName, Guid correlationToken, long version = 0)
            => CommandToAggregate.Commit(
                correlationToken,
                commandName,
                nameof(IChat),
                version);
    }
}
