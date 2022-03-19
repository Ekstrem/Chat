//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using System.Threading.Tasks;
//using Ardalis.Specification;
//using Autofac.Features.Indexed;
//using Chat.Domain.Abstraction;
//using Chat.Domain.Implementation;
//using Chat.DomainServices;
//using Chat.InternalContracts;
//using Chat.Storage.Specifications;
//using Hive.SeedWorks.Monads;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;

//namespace Chat.Storage
//{
//    internal partial class Repository
//    {

//        public async Task Handle(ChatDomainEventCommand notification, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var sw = new Stopwatch();
//                sw.Start();

//                await SaveEventEntity(notification, cancellationToken);

//                sw.Stop();

//                _logger.LogInformation($"Save domain event {notification.AggregateId}/{notification.AggregateVersion} at {sw.ElapsedMilliseconds} ms",
//                    notification.Command.CorrelationToken);

//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, e.Message);
//            }
//        }

//        private async Task SaveEventEntity(ChatDomainEventCommand notification, CancellationToken cancellationToken)
//        {
//            var entry = new DomainEventEntry
//            {
//                AggregateId = notification.AggregateId,
//                AggregateVersion = notification.AggregateVersion,
//                Version = notification.Version,
//                CorrelationToken = notification.CorrelationToken,
//                CommandName = notification.CommandName,
//                SubjectName = notification.SubjectName.Truncate(50),
//                ValueObjects = notification.ToAnemicModel()
//                    .GetAllValueObjects()
//                    .PipeTo(JsonConvert.SerializeObject),
//                Result = notification.Result,
//                Reason = notification.Reason
//            };

//            _dbContext.Set<DomainEventEntry>()
//                .Add(entry);

//            await _dbContext.SaveChangesAsync(cancellationToken);
//        }
//    }
//}
