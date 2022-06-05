using Chat.Application.Queries;
using Hive.SeedWorks.Monads;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueriesController : Controller
    {
        private readonly IMediator _mediator;

        public QueriesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{id}/Info")]
        public async Task<IActionResult> GetInfoAsync(Guid id)
            => new GetChatInfoQuery(id, default)
                .PipeTo(async query => await _mediator.Send(query))
                .PipeTo(r => r.GetAwaiter().GetResult())
                .Either(r => r != null, Ok, _ => (IActionResult)new NotFoundResult());
    }
}
