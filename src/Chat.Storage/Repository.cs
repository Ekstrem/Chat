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

namespace Chat.Storage
{
    internal class Repository : IRepository
{
        private readonly ILogger<Repository> _logger;
        private readonly IQueryRepository<DomainEventEventEntry> _eventRepository;

        public Repository(
            ILogger<Repository> logger
          //  ,IQueryRepository<DomainEventEventEntry> eventRepository
            //,IIndex<string, DbContext> dbContexts
            )
        {
            _logger = logger;
           // _eventRepository = eventRepository;
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

        private Task<List<IChatAnemicModel>> AggregateStreams(Guid? defaultStreamId,
            ISpecification<DomainEventEventEntry> spec, CancellationToken cancellationToken)
        {
            return Task.FromResult( new List<IChatAnemicModel>());
            /*
            try
            {
                if (spec == null)
                {
                    throw new ArgumentException("Filter spex is empty");
                }

                var sw = new Stopwatch()
                    .Do(s => s.Start());

                var anemicModel = await GetDefaultAnemicModel(defaultStreamId ?? Guid.NewGuid());

                var streamEvents = await _eventRepository
                    .FindByAsync(spec, cancellationToken);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            */
        }
    }
}
