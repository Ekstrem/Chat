using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.Extensions.Logging;

namespace Chat.Storage.ReadModel
{
    /// <summary>
    /// Обработчик проекций — маппит доменные события на ChatReadModel
    /// и сохраняет в Read Store через IReadModelStore.
    /// </summary>
    public class ChatProjectionHandler : IDomainEventHandler<IChat>, IObserver<IDomainEvent<IChat>>
    {
        private readonly IReadModelStore<IChat, ChatReadModel> _store;
        private readonly IReadRepository<IChat, ChatReadModel> _reader;
        private readonly ILogger<ChatProjectionHandler> _logger;

        public ChatProjectionHandler(
            IReadModelStore<IChat, ChatReadModel> store,
            IReadRepository<IChat, ChatReadModel> reader,
            ILogger<ChatProjectionHandler> logger)
        {
            _store = store;
            _reader = reader;
            _logger = logger;
        }

        public void OnCompleted() { }
        public void OnError(Exception error)
        {
            _logger.LogError(error, "Ошибка в потоке проекций ChatReadModel.");
        }

        public void OnNext(IDomainEvent<IChat> domainEvent)
        {
            ProjectAsync(domainEvent, CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Проецирует доменное событие на ChatReadModel.
        /// Идемпотентно: пропускает событие, если Version уже обработана.
        /// </summary>
        public async Task ProjectAsync(IDomainEvent<IChat> domainEvent, CancellationToken ct)
        {
            var commandName = domainEvent.Command.CommandName;
            var aggregateId = domainEvent.Id;

            // Загрузить существующую проекцию (или null)
            ChatReadModel existing = null;
            try
            {
                existing = await _reader.GetByIdAsync(aggregateId, ct);
            }
            catch
            {
                // Проекция ещё не существует — создадим новую
            }

            // Идемпотентность: если Version уже обработана — пропустить
            if (existing != null && existing.Version >= domainEvent.Version)
            {
                _logger.LogDebug(
                    "Проекция {AggregateId} уже имеет версию {ExistingVersion} >= {EventVersion}, пропуск.",
                    aggregateId, existing.Version, domainEvent.Version);
                return;
            }

            var readModel = existing ?? new ChatReadModel
            {
                Id = aggregateId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Применить изменения на основе команды
            ApplyEvent(readModel, domainEvent, commandName);

            // Обновить метаданные
            readModel.LastCommand = commandName;
            readModel.Version = domainEvent.Version;
            readModel.UpdatedAt = DateTimeOffset.UtcNow;

            await _store.UpsertAsync(readModel, ct);

            _logger.LogInformation(
                "Проекция обновлена: Id={AggregateId}, Command={Command}, Version={Version}",
                aggregateId, commandName, domainEvent.Version);
        }

        /// <summary>
        /// Проецирует событие из Event Store (DomainEventEventEntry) при rebuild.
        /// </summary>
        public async Task ProjectFromEntryAsync(DomainEventEventEntry entry, CancellationToken ct)
        {
            var aggregateId = entry.Id;

            ChatReadModel existing = null;
            try
            {
                existing = await _reader.GetByIdAsync(aggregateId, ct);
            }
            catch
            {
                // Проекция не существует
            }

            // Идемпотентность
            if (existing != null && existing.Version >= entry.Version)
                return;

            var readModel = existing ?? new ChatReadModel
            {
                Id = aggregateId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            ApplyFromEntry(readModel, entry);

            readModel.LastCommand = entry.CommandName;
            readModel.Version = entry.Version;
            readModel.UpdatedAt = DateTimeOffset.UtcNow;

            await _store.UpsertAsync(readModel, ct);
        }

        private static void ApplyEvent(ChatReadModel model, IDomainEvent<IChat> domainEvent, string commandName)
        {
            var changedVOs = domainEvent.ChangedValueObjects;

            switch (commandName)
            {
                case nameof(IChatAggregate.SubscriberRequestQuestion):
                    if (changedVOs.TryGetValue("Root", out var root) && root is IChatRoot chatRoot)
                    {
                        model.UserId = chatRoot.UserId;
                        model.SessionId = chatRoot.SessionId;
                    }
                    if (changedVOs.TryGetValue("Actor", out var actor) && actor is IChatActor chatActor)
                    {
                        model.ActorLogin = chatActor.Login;
                        model.ActorType = chatActor.Type.ToString();
                    }
                    model.Status = "New";
                    model.MessageCount = 1;
                    break;

                case nameof(IChatAggregate.OperatorDequeueRequest):
                    if (changedVOs.TryGetValue("Actor", out var opActor) && opActor is IChatActor opChatActor)
                    {
                        model.OperatorLogin = opChatActor.Login;
                        model.ActorLogin = opChatActor.Login;
                        model.ActorType = opChatActor.Type.ToString();
                    }
                    model.Status = "InProgress";
                    break;

                case nameof(IChatAggregate.OperatorRepliedToMessage):
                case nameof(IChatAggregate.BotRepliedToUser):
                    if (changedVOs.TryGetValue("Actor", out var replyActor) && replyActor is IChatActor replyChatActor)
                    {
                        model.ActorLogin = replyChatActor.Login;
                        model.ActorType = replyChatActor.Type.ToString();
                    }
                    model.MessageCount++;
                    break;

                case nameof(IChatAggregate.SubscriberGaveFeedback):
                    if (changedVOs.TryGetValue("Feedback", out var feedback) && feedback is IChatFeedback chatFeedback)
                    {
                        model.FeedbackText = chatFeedback.Text;
                        model.FeedbackValue = chatFeedback.Value;
                    }
                    break;

                case nameof(IChatAggregate.SessionEndingByTrigger):
                    model.Status = "Closed";
                    break;
            }
        }

        private static void ApplyFromEntry(ChatReadModel model, DomainEventEventEntry entry)
        {
            // При rebuild десериализуем ChangedValueObjectsJson
            if (string.IsNullOrEmpty(entry.ChangedValueObjectsJson))
                return;

            JsonElement root;
            try
            {
                root = JsonDocument.Parse(entry.ChangedValueObjectsJson).RootElement;
            }
            catch
            {
                return;
            }

            switch (entry.CommandName)
            {
                case nameof(IChatAggregate.SubscriberRequestQuestion):
                    if (root.TryGetProperty("Root", out var rootEl))
                    {
                        if (rootEl.TryGetProperty("UserId", out var userId))
                            model.UserId = Guid.Parse(userId.GetString()!);
                        if (rootEl.TryGetProperty("SessionId", out var sessionId))
                            model.SessionId = sessionId.GetInt32();
                    }
                    if (root.TryGetProperty("Actor", out var actorEl))
                    {
                        if (actorEl.TryGetProperty("Login", out var login))
                            model.ActorLogin = login.GetString()!;
                        if (actorEl.TryGetProperty("Type", out var type))
                            model.ActorType = ((UserType)type.GetInt32()).ToString();
                    }
                    model.Status = "New";
                    model.MessageCount = 1;
                    break;

                case nameof(IChatAggregate.OperatorDequeueRequest):
                    if (root.TryGetProperty("Actor", out var opEl))
                    {
                        if (opEl.TryGetProperty("Login", out var login))
                        {
                            model.OperatorLogin = login.GetString()!;
                            model.ActorLogin = login.GetString()!;
                        }
                        if (opEl.TryGetProperty("Type", out var type))
                            model.ActorType = ((UserType)type.GetInt32()).ToString();
                    }
                    model.Status = "InProgress";
                    break;

                case nameof(IChatAggregate.OperatorRepliedToMessage):
                case nameof(IChatAggregate.BotRepliedToUser):
                    if (root.TryGetProperty("Actor", out var replyEl))
                    {
                        if (replyEl.TryGetProperty("Login", out var login))
                            model.ActorLogin = login.GetString()!;
                        if (replyEl.TryGetProperty("Type", out var type))
                            model.ActorType = ((UserType)type.GetInt32()).ToString();
                    }
                    model.MessageCount++;
                    break;

                case nameof(IChatAggregate.SubscriberGaveFeedback):
                    if (root.TryGetProperty("Feedback", out var fbEl))
                    {
                        if (fbEl.TryGetProperty("Text", out var text))
                            model.FeedbackText = text.GetString();
                        if (fbEl.TryGetProperty("Value", out var value))
                            model.FeedbackValue = value.GetByte();
                    }
                    break;

                case nameof(IChatAggregate.SessionEndingByTrigger):
                    model.Status = "Closed";
                    break;
            }
        }
    }
}
