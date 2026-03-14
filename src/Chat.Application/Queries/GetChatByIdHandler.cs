using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Storage;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Queries
{
    public class GetChatByIdHandler : IRequestHandler<GetChatByIdQuery, ChatOperationResult>
    {
        private readonly CommandDbContext _dbContext;
        private readonly IEventStoreRepository<IChat, IChatAnemicModel> _eventStore;

        public GetChatByIdHandler(
            CommandDbContext dbContext,
            IEventStoreRepository<IChat, IChatAnemicModel> eventStore)
        {
            _dbContext = dbContext;
            _eventStore = eventStore;
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

            // Fallback на event store через библиотеку
            var events = await _eventStore.GetById(request.AggregateId, cancellationToken);

            if (events == null || events.Count == 0)
            {
                return ChatOperationResult.Failure("Chat not found.");
            }

            var latest = events[events.Count - 1];
            return ChatOperationResult.Success(
                request.AggregateId,
                latest.Version,
                "Restored from event store");
        }
    }
}
