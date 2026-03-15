# Materialized Views: Chat

> Сущности на уровне БД, проецируемые из доменных событий.
> Атрибуты коррелируют с Query API Response DTO (артефакт 4).
> Каждая MV обслуживает конкретный сценарий чтения.

## Обзор проекций

| # | Проекция (таблица) | Назначение | Обслуживает Query API |
|---|-------------------|-----------|----------------------|
| 1 | `chat_sessions` | Основная проекция сессии | GET /chat/{id}, GET /chat?userId= |
| 2 | `chat_queue` | Очередь необработанных обращений | GET /chat?status=active&operator=unassigned |
| 3 | `chat_messages` | Денормализованная история сообщений | GET /chat/{id}/messages |
| 4 | `chat_stats` | Агрегированная статистика | GET /chat/stats |

## 1. chat_sessions (основная проекция)

**Схема:** `read_model`  
**Read Model класс:** `ChatSessionReadModel : IReadModel<IChat>`

| Колонка | Тип | Источник (событие → поле) | Query API DTO |
|---------|-----|--------------------------|--------------|
| `id` | UUID PK | ChatSessionCreated → AggregateId | ChatDetailResponse.id |
| `user_id` | UUID | ChatSessionCreated → Root.UserId | ChatDetailResponse.userId |
| `session_id` | INT | ChatSessionCreated → Root.SessionId | ChatDetailResponse.sessionId |
| `status` | TEXT | Вычисляемый: New→Active→InProgress→Closed | ChatDetailResponse.status |
| `subscriber_login` | TEXT | ChatSessionCreated → Actor.Login | ChatDetailResponse.subscriberLogin |
| `operator_login` | TEXT NULL | RequestDequeued → Actor.Login | ChatDetailResponse.operatorLogin |
| `message_count` | INT | +1 при каждом *Replied / *Clarified / *Media | ChatDetailResponse.messageCount |
| `last_message_at` | TIMESTAMPTZ | Каждое событие с Message → Event.CreatedAt | ChatDetailResponse.lastMessageAt |
| `created_at` | TIMESTAMPTZ | ChatSessionCreated → Event.CreatedAt | ChatDetailResponse.createdAt |
| `closed_at` | TIMESTAMPTZ NULL | SessionClosed → Event.CreatedAt | ChatDetailResponse.closedAt |
| `feedback_type` | TEXT NULL | FeedbackReceived → Feedback.Type | ChatDetailResponse.feedback.type |
| `feedback_value` | SMALLINT NULL | FeedbackReceived → Feedback.Value | ChatDetailResponse.feedback.value |
| `feedback_text` | TEXT NULL | FeedbackReceived → Feedback.Text | ChatDetailResponse.feedback.text |
| `version` | BIGINT | Последнее событие → Version | (idempotency key) |

**Индексы:**
- `PK: id`
- `IX_user_id: user_id` — поиск сессий абонента
- `IX_status: status WHERE status != 'Closed'` — фильтр активных

**Проецирование из событий:**

| Доменное событие | Действие в chat_sessions |
|-----------------|--------------------------|
| ChatSessionCreated | INSERT: id, user_id, session_id, status='Active', subscriber_login, message_count=1, created_at |
| BotReplied | UPDATE: message_count++, last_message_at |
| RequestDequeued | UPDATE: operator_login, status='InProgress' |
| OperatorReplied | UPDATE: message_count++, last_message_at |
| QuestionClarified | UPDATE: message_count++, last_message_at |
| FeedbackReceived | UPDATE: feedback_type, feedback_value, feedback_text |
| MediaUploaded | UPDATE: message_count++, last_message_at |
| SessionClosed | UPDATE: status='Closed', closed_at |

## 2. chat_queue (очередь для операторов)

**Схема:** `read_model`  
**Read Model класс:** `ChatQueueReadModel : IReadModel<IChat>`

| Колонка | Тип | Источник | Query API DTO |
|---------|-----|---------|--------------|
| `id` | UUID PK | AggregateId | ChatQueueResponse.items[].id |
| `subscriber_login` | TEXT | ChatSessionCreated → Actor.Login | .subscriberLogin |
| `question_preview` | TEXT | ChatSessionCreated → Message.Text (обрезано до 200 символов) | .questionPreview |
| `platform` | TEXT | ChatSessionCreated → Message.Platform | .platform |
| `created_at` | TIMESTAMPTZ | ChatSessionCreated → Event.CreatedAt | .waitingSince |
| `bot_replied` | BOOL | BotReplied → true | .botReplied |

**Индексы:**
- `PK: id`
- `IX_created_at: created_at` — сортировка очереди по времени ожидания

**Проецирование:**

| Доменное событие | Действие |
|-----------------|---------|
| ChatSessionCreated | INSERT: id, subscriber_login, question_preview, platform, created_at, bot_replied=false |
| BotReplied | UPDATE: bot_replied=true |
| RequestDequeued | **DELETE** (обращение взято — убираем из очереди) |
| SessionClosed | **DELETE** (сессия закрыта — убираем из очереди) |

**Особенность:** эта проекция убывающая — записи удаляются при взятии обращения или закрытии.

## 3. chat_messages (история сообщений)

**Схема:** `read_model`  
**Read Model класс:** `ChatMessageReadModel : IReadModel<IChat>`

| Колонка | Тип | Источник | Query API DTO |
|---------|-----|---------|--------------|
| `id` | UUID PK | auto-generated | — |
| `chat_id` | UUID FK | AggregateId | ChatMessagesResponse.chatId |
| `actor_login` | TEXT | Event → Actor.Login | .messages[].actorLogin |
| `actor_type` | TEXT | Event → Actor.Type | .messages[].actorType |
| `text` | TEXT | Event → Message.Text | .messages[].text |
| `message_type` | TEXT | Event → Message.Type | .messages[].type |
| `content_id` | UUID NULL | Event → Message.ContentId | .messages[].contentId |
| `sent_at` | TIMESTAMPTZ | Event.CreatedAt | .messages[].sentAt |

**Индексы:**
- `PK: id`
- `IX_chat_id_sent_at: (chat_id, sent_at)` — хронология сообщений

**Проецирование:**

| Доменное событие | Действие |
|-----------------|---------|
| ChatSessionCreated | INSERT (первое сообщение абонента) |
| BotReplied | INSERT |
| OperatorReplied | INSERT |
| QuestionClarified | INSERT |
| MediaUploaded (subscriber) | INSERT |
| MediaUploaded (operator) | INSERT |

**Особенность:** append-only — сообщения только добавляются, не обновляются.

## 4. chat_stats (агрегированная статистика)

**Схема:** `read_model`  
**Read Model класс:** `ChatStatsReadModel : IReadModel<IChat>`

| Колонка | Тип | Источник | Query API DTO |
|---------|-----|---------|--------------|
| `date` | DATE PK | Event.CreatedAt (дата) | ChatStatsResponse.period |
| `total_sessions` | INT | +1 при ChatSessionCreated | .totalSessions |
| `closed_sessions` | INT | +1 при SessionClosed | .sessionsByStatus.closed |
| `active_sessions` | INT | +1/-1 при Created/Closed | .sessionsByStatus.active |
| `in_progress_sessions` | INT | +1/-1 при Dequeued/Closed | .sessionsByStatus.inProgress |
| `total_response_time_sec` | BIGINT | Сумма (Dequeued.CreatedAt - Created.CreatedAt) | (для avg) |
| `dequeued_count` | INT | +1 при RequestDequeued | (для avg) |
| `total_feedback_score` | INT | Сумма Feedback.Value | (для avg) |
| `feedback_count` | INT | +1 при FeedbackReceived | (для avg) |

**Вычисляемые поля в Query Handler:**
- `avgResponseTimeSec` = total_response_time_sec / dequeued_count
- `avgFeedbackScore` = total_feedback_score / feedback_count

**Индексы:**
- `PK: date`

**Проецирование:**

| Доменное событие | Действие |
|-----------------|---------|
| ChatSessionCreated | UPSERT: total_sessions++, active_sessions++ |
| RequestDequeued | UPSERT: in_progress_sessions++, active_sessions--, total_response_time_sec += delta |
| SessionClosed | UPSERT: closed_sessions++, active_sessions-- или in_progress_sessions-- |
| FeedbackReceived | UPSERT: total_feedback_score += value, feedback_count++ |

## Корреляция артефактов 3→5→4

```
Command API (артефакт 3)         Materialized Views (артефакт 5)    Query API (артефакт 4)
───────────────────────         ──────────────────────────────     ──────────────────────

SubscriberRequestQuestion  ──►  chat_sessions.INSERT          ──►  ChatDetailResponse
                                chat_queue.INSERT             ──►  ChatQueueResponse
                                chat_messages.INSERT          ──►  ChatMessagesResponse
                                chat_stats.UPSERT             ──►  ChatStatsResponse

OperatorDequeueRequest     ──►  chat_sessions.UPDATE(status)  ──►  ChatDetailResponse
                                chat_queue.DELETE             ──►  (убрано из очереди)
                                chat_stats.UPSERT             ──►  ChatStatsResponse

OperatorRepliedToMessage   ──►  chat_sessions.UPDATE(count)   ──►  ChatDetailResponse
                                chat_messages.INSERT          ──►  ChatMessagesResponse

SessionEndingByTrigger     ──►  chat_sessions.UPDATE(closed)  ──►  ChatDetailResponse
                                chat_queue.DELETE             ──►  (убрано из очереди)
                                chat_stats.UPSERT             ──►  ChatStatsResponse
```
