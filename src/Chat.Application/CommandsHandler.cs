using AutoMapper;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Monads;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Commands
{
    internal class CommandsHandler :
        IRequestHandler<RequestQuestionCommand, Guid>
    {
        private readonly IChatAggregateProvider _provider;
        private readonly IMapper _mapper;

        public CommandsHandler(IChatAggregateProvider provider, IMapper mapper)
        {
            _provider = provider;
            _mapper = mapper;
        }

        public Task<Guid> Handle(RequestQuestionCommand request, CancellationToken cancellationToken)
        {
            var model = AnemicModel
                    .Create(
                       Guid.NewGuid(),
                        CreateCommandMetadata(
                            nameof(Aggregate.SubscriberRequestQuestion),
                            request.CorrelationId,
                            DateTime.UtcNow.Ticks),
                        ChatRoot.CreateInstance(
                            Guid.NewGuid(),
                            23453254),
                        ChatActor.CreateInstance(
                            request.UserName,
                            (byte)UserType.Client),
                        default,
                        new[]
                        {
                            ChatMessage
                                .CreateInstance(
                                    request.Type,
                                    request.Message,
                                    Platform.Android,
                                    request.Application,
                                    Guid.Empty)
                        });

            return _provider.CreateAggregate()
                .PipeTo(aggregate => aggregate.SubscriberRequestQuestion(model))
                .PipeTo(result => result.BusinessOperationData.Model.Id)
                .PipeTo(Task.FromResult);
        }

        private ICommandToAggregate CreateCommandMetadata(string commandName, Guid correlationToken, long version = 0)
           => CommandToAggregate.Commit(
               correlationToken,
               commandName,
               nameof(IChat),
               version);
    }
}
