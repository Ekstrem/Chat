using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Storage.ReadModels;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Storage.Projections
{
    public class ChatProjectionService
    {
        private readonly CommandDbContext _dbContext;
        private readonly ILogger<ChatProjectionService> _logger;

        public ChatProjectionService(
            CommandDbContext dbContext,
            ILogger<ChatProjectionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ProjectAsync(DomainEventEntry entry, CancellationToken cancellationToken = default)
        {
            var readModel = await _dbContext.ChatReadModels
                .FirstOrDefaultAsync(r => r.Id == entry.Id, cancellationToken);

            if (readModel == null)
            {
                readModel = new ChatReadModel
                {
                    Id = entry.Id,
                    Version = entry.Version,
                    Status = "Active",
                    LastCommandName = entry.CommandName,
                    CreatedAt = entry.CreatedAt,
                    UpdatedAt = entry.CreatedAt
                };
                _dbContext.ChatReadModels.Add(readModel);
                _logger.LogDebug("Created read model for aggregate {AggregateId}", entry.Id);
            }
            else
            {
                readModel.Version = entry.Version;
                readModel.LastCommandName = entry.CommandName;
                readModel.UpdatedAt = DateTime.UtcNow;

                UpdateStatusFromCommand(readModel, entry.CommandName);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogDebug(
                "Projected event for aggregate {AggregateId}, version {Version}, status {Status}",
                entry.Id, entry.Version, readModel.Status);
        }

        private static void UpdateStatusFromCommand(ChatReadModel model, string commandName)
        {
            switch (commandName)
            {
                case "OperatorDequeueRequest":
                    model.Status = "Dequeued";
                    break;
                case "SessionEndingByTrigger":
                    model.Status = "Ended";
                    break;
                case "OperatorRepliedToMessage":
                case "BotRepliedToUser":
                    model.MessageCount++;
                    break;
            }
        }
    }
}
