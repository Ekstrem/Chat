# Query API: Chat v1

> API для потребителей, запрашивающих состояние.
> Атрибуты Response DTO коррелируют с Materialized Views (артефакт 5).
> Query Side не обращается к Domain и Event Store — только к Read Store.

## HTTP-эндпоинты (Query)

| # | Метод | URI | Описание | Response DTO |
|---|-------|-----|----------|-------------|
| 1 | GET | `/api/v1/chat/{id}` | Получить сессию чата по ID | `ChatDetailResponse` |
| 2 | GET | `/api/v1/chat?userId={userId}` | Активные сессии абонента | `ChatListResponse` |
| 3 | GET | `/api/v1/chat?status=active&operator=unassigned` | Очередь необработанных обращений | `ChatQueueResponse` |
| 4 | GET | `/api/v1/chat/{id}/messages` | История сообщений сессии | `ChatMessagesResponse` |
| 5 | GET | `/api/v1/chat/stats?from={date}&to={date}` | Статистика за период | `ChatStatsResponse` |

## Response DTO

### 1. ChatDetailResponse (полная сессия)

```
{
  "id":             Guid,          ← MV: chat_sessions.id
  "userId":         Guid,          ← MV: chat_sessions.user_id
  "sessionId":      int,           ← MV: chat_sessions.session_id
  "status":         string,        ← MV: chat_sessions.status
  "subscriberLogin": string,       ← MV: chat_sessions.subscriber_login
  "operatorLogin":  string | null, ← MV: chat_sessions.operator_login
  "messageCount":   int,           ← MV: chat_sessions.message_count
  "lastMessageAt":  DateTime,      ← MV: chat_sessions.last_message_at
  "createdAt":      DateTime,      ← MV: chat_sessions.created_at
  "closedAt":       DateTime | null, ← MV: chat_sessions.closed_at
  "feedback": {                    ← MV: chat_sessions.feedback_*
    "type":  string | null,
    "value": byte | null,
    "text":  string | null
  }
}
```

**Корреляция с MV:** каждое поле Response DTO соответствует колонке `chat_sessions`.

### 2. ChatListResponse (список сессий абонента)

```
{
  "items": [
    {
      "id":            Guid,       ← MV: chat_sessions.id
      "status":        string,     ← MV: chat_sessions.status
      "lastMessageAt": DateTime,   ← MV: chat_sessions.last_message_at
      "messageCount":  int,        ← MV: chat_sessions.message_count
      "operatorLogin": string | null ← MV: chat_sessions.operator_login
    }
  ],
  "total": int
}
```

### 3. ChatQueueResponse (очередь для операторов)

```
{
  "items": [
    {
      "id":              Guid,      ← MV: chat_queue.id
      "subscriberLogin": string,    ← MV: chat_queue.subscriber_login
      "questionPreview":  string,   ← MV: chat_queue.question_preview
      "platform":        string,    ← MV: chat_queue.platform
      "waitingSince":    DateTime,  ← MV: chat_queue.created_at
      "botReplied":      bool       ← MV: chat_queue.bot_replied
    }
  ],
  "total": int
}
```

**Корреляция с MV:** атрибуты соответствуют колонкам `chat_queue` — специализированной проекции для операторского UI.

### 4. ChatMessagesResponse (история сообщений)

```
{
  "chatId": Guid,
  "messages": [
    {
      "actorLogin": string,        ← MV: chat_messages.actor_login
      "actorType":  string,        ← MV: chat_messages.actor_type
      "text":       string,        ← MV: chat_messages.text
      "type":       string,        ← MV: chat_messages.message_type
      "contentId":  Guid | null,   ← MV: chat_messages.content_id
      "sentAt":     DateTime       ← MV: chat_messages.sent_at
    }
  ]
}
```

**Корреляция с MV:** соответствует `chat_messages` — денормализованная таблица сообщений.

### 5. ChatStatsResponse (статистика)

```
{
  "period": { "from": DateTime, "to": DateTime },
  "totalSessions":      int,      ← MV: chat_stats.total_sessions
  "avgResponseTimeSec": double,   ← MV: chat_stats.avg_response_time_sec
  "avgFeedbackScore":   double,   ← MV: chat_stats.avg_feedback_score
  "sessionsByStatus": {            ← MV: chat_stats.sessions_by_status
    "active":     int,
    "inProgress": int,
    "closed":     int
  }
}
```

**Корреляция с MV:** `chat_stats` — агрегированная проекция для дашборда.

## Соответствие Query DTO → MediatR Query → Read Store

```
GET /api/v1/chat/{id}
  → GetChatByIdQuery : IRequest<ChatDetailResponse>
    → IReadRepository<IChat, ChatSessionReadModel>.GetByIdAsync(id)
      → SELECT * FROM read_model.chat_sessions WHERE id = @id

GET /api/v1/chat?status=active&operator=unassigned
  → GetChatQueueQuery : IRequest<ChatQueueResponse>
    → IReadRepository<IChat, ChatQueueReadModel>.GetAllAsync(filter)
      → SELECT * FROM read_model.chat_queue WHERE status = 'Active' AND operator_login IS NULL
```

## Потребители Query API

| Потребитель | Эндпоинты | Назначение |
|------------|-----------|-----------|
| UI абонента | 1, 2, 4 | Просмотр своих сессий и истории |
| UI оператора | 1, 3, 4 | Очередь обращений, работа с сессией |
| Дашборд супервизора | 3, 5 | Мониторинг очереди и статистики |
| Внешние интеграции | 1, 5 | API для партнёров |
