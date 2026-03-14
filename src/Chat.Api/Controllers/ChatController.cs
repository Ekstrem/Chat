using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Application;
using Chat.Application.Commands;
using Chat.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Subscriber requests a new question in chat.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ChatOperationResult), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SubscriberRequestQuestion(
            [FromBody] SubscriberRequestQuestionCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Operator dequeues a chat request.
        /// </summary>
        [HttpPost("{id:guid}/dequeue")]
        [ProducesResponseType(typeof(ChatOperationResult), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> OperatorDequeueRequest(
            [FromRoute] Guid id,
            [FromBody] OperatorDequeueRequestCommand command,
            CancellationToken cancellationToken)
        {
            command.AggregateId = id;
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Operator replies to a message.
        /// </summary>
        [HttpPost("{id:guid}/reply")]
        [ProducesResponseType(typeof(ChatOperationResult), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> OperatorRepliedToMessage(
            [FromRoute] Guid id,
            [FromBody] OperatorRepliedToMessageCommand command,
            CancellationToken cancellationToken)
        {
            command.AggregateId = id;
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get chat by identifier.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ChatOperationResult), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetChatById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetChatByIdQuery { AggregateId = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}
