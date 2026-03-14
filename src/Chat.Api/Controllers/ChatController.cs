using Chat.Application;
using Chat.Application.Commands;
using Chat.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        /// Абонент задал вопрос в чате.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ChatOperationResult>> SubscriberRequestQuestion(
            [FromBody] SubscriberRequestQuestionCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Оператор взял обращение на обработку.
        /// </summary>
        [HttpPost("{id:guid}/dequeue")]
        public async Task<ActionResult<ChatOperationResult>> OperatorDequeueRequest(
            Guid id,
            [FromBody] OperatorDequeueRequestCommand command,
            CancellationToken cancellationToken)
        {
            command.AggregateId = id;
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Оператор ответил на обращение.
        /// </summary>
        [HttpPost("{id:guid}/reply")]
        public async Task<ActionResult<ChatOperationResult>> OperatorRepliedToMessage(
            Guid id,
            [FromBody] OperatorRepliedToMessageCommand command,
            CancellationToken cancellationToken)
        {
            command.AggregateId = id;
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Сессия завершилась по триггеру.
        /// </summary>
        [HttpPost("{id:guid}/end")]
        public async Task<ActionResult<ChatOperationResult>> SessionEndingByTrigger(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new SessionEndingByTriggerCommand { AggregateId = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить чат по идентификатору.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetChatById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetChatByIdQuery { AggregateId = id }, cancellationToken);
            return Ok(result);
        }
    }
}
