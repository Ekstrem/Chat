using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chat.DomainServices;
using Chat.InternalContracts;
using Hive.SeedWorks.Monads;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Chat.Storage
{
    internal partial class Repository
    {

        public async Task Handle(ChatDomainEventCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                await SaveEventEntity(notification, cancellationToken);

                sw.Stop();

                _logger.LogInformation($"Save domain event {notification.Id}/{notification.Version} at {sw.ElapsedMilliseconds} ms",
                    notification.Command.CorrelationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        private async Task SaveEventEntity(ChatDomainEventCommand notification, CancellationToken cancellationToken)
        {
            var entry = new DomainEventEntry
            {
                Id = Guid.NewGuid(),
                AggregateId = notification.Id,
                AggregateVersion = notification.Version,
                Version = notification.Command.Version,
                CorrelationToken = notification.Command.CorrelationToken,
                CommandName = notification.Command.CommandName,
                SubjectName = notification.Command.SubjectName,
                ValueObjects = notification
                    .ChangedValueObjects
                    .PipeTo(JsonConvert.SerializeObject),
                Result = notification.Result,
                Reason = notification.Reason
            };

            _dbContext
                .Set<DomainEventEntry>()
                .Add(entry);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
