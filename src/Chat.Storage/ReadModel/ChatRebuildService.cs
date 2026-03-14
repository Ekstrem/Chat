using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Storage.ReadModel
{
    /// <summary>
    /// Перестраивает все проекции ChatReadModel из Event Store (Command DB).
    /// Читает события в хронологическом порядке и прогоняет через ChatProjectionHandler.
    /// Идемпотентно: события с версией <= текущей версии проекции пропускаются.
    /// </summary>
    public class ChatRebuildService : IRebuildService<IChat>
    {
        private readonly CommandDbContext _commandDb;
        private readonly ChatProjectionHandler _projectionHandler;
        private readonly ILogger<ChatRebuildService> _logger;

        public ChatRebuildService(
            CommandDbContext commandDb,
            ChatProjectionHandler projectionHandler,
            ILogger<ChatRebuildService> logger)
        {
            _commandDb = commandDb;
            _projectionHandler = projectionHandler;
            _logger = logger;
        }

        public async Task RebuildAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Rebuild проекций ChatReadModel: начало.");

            var events = await _commandDb.Events
                .OrderBy(e => e.CreatedAt)
                .ThenBy(e => e.Version)
                .ToListAsync(ct);

            _logger.LogInformation("Rebuild: найдено {Count} событий в Event Store.", events.Count);

            var projected = 0;
            foreach (var entry in events)
            {
                await _projectionHandler.ProjectFromEntryAsync(entry, ct);
                projected++;

                if (projected % 100 == 0)
                {
                    _logger.LogInformation("Rebuild: обработано {Count}/{Total} событий.",
                        projected, events.Count);
                }
            }

            _logger.LogInformation("Rebuild проекций ChatReadModel завершён: {Count} событий обработано.", projected);
        }
    }
}
