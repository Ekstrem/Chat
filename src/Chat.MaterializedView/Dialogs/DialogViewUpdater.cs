using Autofac.Features.Indexed;
using Chat.InternalContracts;
using Chat.MaterializedView.Dialogs.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hive.SeedWorks.Monads;
using Chat.Domain.Abstraction;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Chat.MaterializedView.Dialogs
{
    internal class DialogViewUpdater : INotificationHandler<ChatDomainEventCommand>
    {
        private readonly ILogger<DialogViewUpdater> _logger;
        private readonly IQueryRepository<DialogView> _eventRepository;
        private readonly DbContext _dbContext;

        public DialogViewUpdater(
            ILogger<DialogViewUpdater> logger,
            IQueryRepository<DialogView> eventRepository,
            IIndex<string, DbContext> dbContexts
        )
        {
            _logger = logger;
            _eventRepository = eventRepository;

            if (!dbContexts.TryGetValue(nameof(MaterializedView), out _dbContext))
            {
                _logger.Log(LogLevel.Warning, $"{nameof(MaterializedView)} not injected at {nameof(DialogViewUpdater)}");
            }
        }

        public async Task Handle(ChatDomainEventCommand notification, CancellationToken cancellationToken)
        {
            var anemicModel = notification.ToAnemicModel();

            var view = new GetByIdSpec(anemicModel.Id)
                .PipeTo(spec => _eventRepository.FindByAsync(spec, cancellationToken))
                .PipeTo(r => r.GetAwaiter().GetResult());

            if (view == null)
            {
                await CreateView(anemicModel);
            }
            else
            {
                await UpdateView(view, anemicModel);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private Task CreateView(IChatAnemicModel anemicModel)
        {
            var entry = new DialogView
            {
                AggregateId = anemicModel.Id,
                Name = anemicModel.Actor.Login,
            };

            _dbContext
                .Set<DialogView>()
                .Add(entry);

            return Task.CompletedTask;
        }

        private Task UpdateView(DialogView view, IChatAnemicModel anemicModel)
        {
            return Task.CompletedTask;
        }
    }
}
