using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Autofac.Features.Indexed;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Chat.DomainServices;
using Chat.InternalContracts;
using Chat.Storage.Specifications;
using Hive.SeedWorks.Monads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Chat.Storage
{
    internal partial class Repository : 
        IRepository,
        INotificationHandler<ChatDomainEventCommand>
    {
        private readonly ILogger<Repository> _logger;
        private readonly IQueryRepository<DomainEventEntry> _eventRepository;
        private readonly DbContext _dbContext;

        public Repository(
            ILogger<Repository> logger,
            IQueryRepository<DomainEventEntry> eventRepository,
            IIndex<string, DbContext> dbContexts
        )
        {
            _logger = logger;
            _eventRepository = eventRepository;

            if (!dbContexts.TryGetValue(nameof(Storage), out _dbContext))
            {
                _logger.Log(LogLevel.Warning, $"{nameof(Storage)} not injected at {nameof(Repository)}");
            }
        }
   
        public Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
            => new GetByIdSpec(id)
                .PipeTo(query => AggregateStreams(id, query, cancellationToken));


        public Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
        {
            var aggregates = new GetByIdAndVersionSpec(id, version)
                .PipeTo(query => AggregateStreams(id, query, cancellationToken))
                .PipeTo(r => r.Result);

            var result = aggregates.SingleOrDefault() ?? DefaultAnemicModel.Create(Guid.NewGuid());
            return Task.FromResult(result);
        }

        public Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
        {
            var aggregates = new GetByCorrelationTokenSpec(correlationToken)
                .PipeTo(query => AggregateStreams(Guid.NewGuid(), query, cancellationToken))
                .PipeTo(r => r.Result);

            var result = aggregates.SingleOrDefault() ?? DefaultAnemicModel.Create(Guid.NewGuid());
            return Task.FromResult(result);
        }

        private async Task<List<IChatAnemicModel>> AggregateStreams(Guid? defaultStreamId,
            ISpecification<DomainEventEntry> spec, CancellationToken cancellationToken)
        {
            try
            {
                if (spec == null)
                {
                    throw new ArgumentException("Filter spec is empty");
                }

                var anemicModel = GetDefaultAnemicModel(defaultStreamId ?? Guid.NewGuid());

                var streamEvents = await _eventRepository.ListAsync(spec, cancellationToken);

                if (!streamEvents.Any())
                {
                    return new List<IChatAnemicModel> { anemicModel };
                }

                var list = new List<IChatAnemicModel>();
                foreach (var @event in streamEvents)
                {
                    anemicModel = AppendEvent(@event, anemicModel);
                    list.Add(anemicModel);
                }

                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IChatAnemicModel GetDefaultAnemicModel(Guid streamId)
            => DefaultAnemicModel.Create(streamId);

        private IChatAnemicModel AppendEvent(DomainEventEntry ev, IChatAnemicModel model)
        {
            try
            {
                var changedValueObjects = ev.ChangedValueObjects;
                var root = changedValueObjects.TryGetValue(nameof(IChatAnemicModel.Root), out var rawRoot)
                           && rawRoot is ChatRoot rootValueObject
                    ? rootValueObject
                    : model.Root;

                var actor = changedValueObjects.TryGetValue(nameof(IChatAnemicModel.Actor), out var rawActor)
                           && rawActor is ChatActor actorValueObject
                    ? actorValueObject
                    : model.Actor;

                var feedback = changedValueObjects.TryGetValue(nameof(IChatAnemicModel.Feedback), out var rawFeedback)
                            && rawFeedback is ChatFeedback feedbackValueObject
                    ? feedbackValueObject
                    : model.Feedback;

                var messages = new List<IChatMessage>();

                return AnemicModel.Create(ev.AggregateId, ev.Command, root, actor, feedback, messages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
