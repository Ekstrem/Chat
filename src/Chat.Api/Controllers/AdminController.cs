using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IRebuildService<IChat> _rebuildService;

        public AdminController(IRebuildService<IChat> rebuildService)
        {
            _rebuildService = rebuildService;
        }

        /// <summary>
        /// Перестроить все проекции ChatReadModel из Event Store.
        /// Идемпотентная операция — повторный вызов безопасен.
        /// </summary>
        [HttpPost("rebuild-projections")]
        public async Task<IActionResult> RebuildProjections(CancellationToken ct)
        {
            await _rebuildService.RebuildAsync(ct);
            return Ok(new { message = "Rebuild проекций завершён." });
        }
    }
}
