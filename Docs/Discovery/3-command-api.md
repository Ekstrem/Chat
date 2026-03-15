# Command API: Chat v1

> API и события, вызывающие изменения состояния агрегата.
> Атрибуты контрактов и названия методов коррелируют с Aggregate Design Canvas (секция 6).

## HTTP-эндпоинты (Command)

| # | Метод | URI | Команда агрегата | Request DTO | Начальное состояние |
|---|-------|-----|-----------------|-------------|-------------------|
| 1 | POST | `/api/v1/chat` | SubscriberRequestQuestion | `SubscriberRequestQuestionRequest` | New |
| 2 | POST | `/api/v1/chat/{id}/dequeue` | OperatorDequeueRequest | `OperatorDequeueRequest` | Active |
| 3 | POST | `/api/v1/chat/{id}/reply` | OperatorRepliedToMessage | `OperatorReplyRequest` | InProgress |
| 4 | POST | `/api/v1/chat/{id}/clarify` | OperatorClarifiedQuestion | `OperatorClarifyRequest` | InProgress |
| 5 | POST | `/api/v1/chat/{id}/feedback` | SubscriberGaveFeedback | `FeedbackRequest` | InProgress |
| 6 | POST | `/api/v1/chat/{id}/media` | SubscriberDownloadMedia | `MediaUploadRequest` | Active / InProgress |
| 7 | POST | `/api/v1/chat/{id}/operator-media` | OperatorDownloadMedia | `MediaUploadRequest` | InProgress |
| 8 | POST | `/api/v1/chat/{id}/end` | SessionEndingByTrigger | — | Active / InProgress |

## Входящие события через шину (Command)

| # | Событие из другого контекста | Команда агрегата | Источник |
|---|----------------------------|-----------------|---------|
| 9 | BotReplyReceived | BotRepliedToUser | Bot-сервис |
| 10 | SessionTimeoutTriggered | SessionEndingByTrigger | Триггер-сервис |

## Request DTO

### 1. SubscriberRequestQuestionRequest (создание сессии)

```
{
  "userId":      Guid,       ← Root.UserId
  "sessionId":   int,        ← Root.SessionId
  "login":       string,     ← Actor.Login
  "messageText": string,     ← Message.Text
  "messageType": string,     ← Message.Type ("Text" | "Voice" | "Video")
  "platform":    string,     ← Message.Platform ("Android" | "Windows" | "Linux")
  "application": string      ← Message.Application ("Site" | "Application" | "IncomingCall")
}
```

**Корреляция с VO:** Root (UserId, SessionId) + Actor (Login, Type=Client implicit) + Message (Type, Text, Platform, Application)

### 2. OperatorDequeueRequest (взятие обращения)

```
{
  "operatorLogin": string    ← Actor.Login
}
```

**Корреляция с VO:** Actor (Login, Type=Operator implicit). AggregateId из URI.

### 3. OperatorReplyRequest (ответ оператора)

```
{
  "messageText": string      ← Message.Text
}
```

**Корреляция с VO:** Message (Text, Type=Text implicit). AggregateId из URI. Actor берётся из контекста авторизации.

### 4. OperatorClarifyRequest (уточнение вопроса)

```
{
  "messageText": string      ← Message.Text
}
```

**Корреляция с VO:** аналогично OperatorReplyRequest.

### 5. FeedbackRequest (обратная связь)

```
{
  "type":  string,           ← Feedback.Type ("Boolean" | "Scores" | "Free")
  "value": byte,             ← Feedback.Value
  "text":  string            ← Feedback.Text
}
```

**Корреляция с VO:** Feedback (Type, Value, Text).

### 6-7. MediaUploadRequest (загрузка медиа)

```
{
  "messageType": string,     ← Message.Type ("Voice" | "Video")
  "contentId":   Guid,       ← Message.ContentId
  "platform":    string,     ← Message.Platform
  "application": string      ← Message.Application
}
```

**Корреляция с VO:** Message (Type, ContentId, Platform, Application).

### 8. SessionEndingByTrigger

Тело запроса отсутствует. AggregateId из URI.

## Response DTO (единый для всех команд)

```
ChatOperationResponse
{
  "aggregateId": Guid,
  "version":     long,
  "result":      string,           // "Success" | "Exception" | "WithWarnings"
  "reasons":     string[] | null   // причины при Exception/Warnings
}
```

**HTTP-статусы:**
- `200 OK` — result == Success или WithWarnings
- `400 Bad Request` — result == Exception (инвариант нарушен)
- `404 Not Found` — агрегат не найден

## Соответствие Request DTO → MediatR Command → метод агрегата

```
SubscriberRequestQuestionRequest
  → SubscriberRequestQuestionCommand : IRequest<ChatOperationResult>
    → aggregate.SubscriberRequestQuestion(anemicModel)
      → AggregateResult<IChat, IChatAnemicModel>

OperatorDequeueRequest
  → OperatorDequeueRequestCommand : IRequest<ChatOperationResult>
    → aggregate.OperatorDequeueRequest(anemicModel, commandMetadata)
      → AggregateResult<IChat, IChatAnemicModel>
```

Названия коррелируют по всей цепочке: Request DTO → Command → метод агрегата → доменное событие.
