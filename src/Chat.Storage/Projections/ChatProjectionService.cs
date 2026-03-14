using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Storage.ReadModels;
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

        /// <summary>
        /// Проекция события в read model. Принимает примитивы для независимости от СУБД.
        /// </summary>
        public async Task ProjectAsync(
            Guid id,
            long version,
            string commandName,
            DateTime createdAt,
            CancellationToken cancellationToken = default)
        {
            var readModel = await _dbContext.ChatReadModels
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (readModel == null)
            {
                readModel = new ChatReadModel
                {
                    Id = id,
                    Version = version,
                    Status = "Active",
                    LastCommandName = commandName,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt
                };
                _dbContext.ChatReadModels.Add(readModel);
                _logger.LogDebug("Created read model for aggregate {AggregateId}", id);
            }
            else
            {
                readModel.Version = version;
                readModel.LastCommandName = commandName;
                readModel.UpdatedAt = DateTime.UtcNow;

                UpdateStatusFromCommand(readModel, commandName);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogDebug(
                "Projected event for aggregate {AggregateId}, version {Version}, status {Status}",
                id, version, readModel.Status);
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
