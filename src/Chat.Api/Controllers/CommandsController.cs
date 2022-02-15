using Chat.Api.Middlewares.Correlation;
using Chat.Application.Commands;
using Chat.Contracts.Requests;
using Hive.SeedWorks.Monads;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Chat.Api.Controllers
{

    [ApiController]
    [Route("api/commands")]
    public class CommandsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IExecutionContextAccessor _contextAccessor;

        public CommandsController(IMediator mediator, IExecutionContextAccessor contextAccessor)
        {
            _mediator = mediator;
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        /// <summary>
        /// Начать диалог.
        /// </summary>
        [HttpPost("InitDialogByCall")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> StartDialog([FromBody] StartDialogRequest request)
        {
            var command = RequestQuestionCommand.CreateInstanceByIncommingCall(request.UserName, request.Message, _contextAccessor.CorrelationId);
            return (await _mediator.Send(command))
                .PipeTo(dialogId => Created("api/Queries/Info", dialogId));
        }
    }
}
