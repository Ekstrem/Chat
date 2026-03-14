using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Queries
{
    public class GetChatByIdHandler : IRequestHandler<GetChatByIdQuery, ChatOperationResult>
    {
        private readonly CommandDbContext _dbContext;

        public GetChatByIdHandler(CommandDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatOperationResult> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            // Сначала пробуем Read Model
            var readModel = await _dbContext.ChatReadModels
                .FirstOrDefaultAsync(r => r.Id == request.AggregateId, cancellationToken);

            if (readModel != null)
            {
                return ChatOperationResult.Success(
                    readModel.Id,
                    readModel.Version,
                    $"Status: {readModel.Status}, Messages: {readModel.MessageCount}, Operator: {readModel.OperatorName}");
            }

            // Fallback на event store
            var latestEvent = await _dbContext.Events
                .Where(e => e.Id == request.AggregateId)
                .OrderByDescending(e => e.Version)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestEvent == null)
            {
                return ChatOperationResult.Failure("Chat not found.");
            }

            return ChatOperationResult.Success(
                latestEvent.Id,
                latestEvent.Version,
                latestEvent.Result);
        }
    }
}
