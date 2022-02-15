using Chat.Application.Queries;
using Hive.SeedWorks.Monads;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Chat.Api.Controllers
{
    [ApiController]
    [Route("api/queries")]
    public class QueriesController : Controller
    {
        private readonly IMediator _mediator;

        public QueriesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{id}/{version}")]
        public async Task<IActionResult> GetInfo(Guid id, long version = default)
            => new GetChatInfoQuery(id, version)
            .PipeTo(async query => await _mediator.Send(query))
            .Either(Ok, _ => (IActionResult)new NotFoundResult());
    }
}
