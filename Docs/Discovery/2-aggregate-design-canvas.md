# Aggregate Design Canvas: Chat

## 1. Name
- **Имя:** Chat
- **Создаётся когда:** абонент задаёт вопрос (SubscriberRequestQuestion)
- **Завершается когда:** сессия закрыта по триггеру (SessionEndingByTrigger)
- **Lifecycle:** минуты — часы (одна сессия поддержки)

## 2. Description

Агрегат моделирует одну сессию чата поддержки. Защищает целостность взаимодействия абонент ↔ бот ↔ оператор.

### Value Objects

| Value Object | Свойства | Описание |
|-------------|----------|----------|
| **Root** (ChatRoot) | UserId: Guid, SessionId: int | Корень агрегата — идентификация сессии |
| **Message** (ChatMessage) | Type: MessageType, Text: string, Platform: Platform, Application: Application, ContentId: Guid | Сообщение в чате |
| **Actor** (ChatActor) | Login: string, Type: UserType | Участник (абонент / бот / оператор) |
| **Feedback** (ChatFeedback) | Type: AnswerType, Value: byte, Text: string | Оценка работы |

### Перечисления

| Enum | Значения |
|------|---------|
| MessageType | Text, Voice, Video |
| Platform | Linux, Windows, Android |
| Application | Application, Site, IncomingCall |
| UserType | Client, Bot, Operator |
| AnswerType | Boolean, Scores, Free |

## 3. State Transitions

```
         SubscriberRequestQuestion
[New] ──────────────────────────────► [Active]
                                         │
                     BotRepliedToUser    │    OperatorDequeueRequest
                    ◄───────────────────┤───────────────────────► [InProgress]
                    (остаётся Active)    │                            │
                                         │   OperatorRepliedToMessage │
                                         │   OperatorClarifiedQuestion│
                                         │   SubscriberGaveFeedback   │
                                         │   SubscriberDownloadMedia  │
                                         │   OperatorDownloadMedia    │
                                         │   (остаётся InProgress)    │
                                         │                            │
                                         │   SessionEndingByTrigger   │
                                         ├────────────────────────────► [Closed]
                                         │   SessionEndingByTrigger   │
                                         └────────────────────────────► [Closed]
```

## 4. Enforced Invariants

| Инвариант | Описание | Класс валидатора |
|-----------|----------|-----------------|
| Новый агрегат | SubscriberRequestQuestion допустим только для нового (DefaultAnemicModel) | `IsNewSubscriberRequestValidator` |
| Одно взятие | OperatorDequeueRequest невозможен, если оператор уже назначен | `IsNotAlreadyDequeuedValidator` |
| Активная сессия | Операции невозможны на закрытой или несуществующей сессии | `IsSessionActiveValidator` |

## 5. Corrective Policies

| Политика | Условие | Действие |
|---------|---------|---------|
| Таймаут бота | Бот не ответил за N секунд | Триггер-сервис отправляет SessionEndingByTrigger или эскалация на оператора |
| Таймаут оператора | Оператор не ответил за N минут | Уведомление супервизору (внешний сервис) |

## 6. Handled Commands

| Команда | Входные VO | Описание | Начальное состояние |
|---------|-----------|----------|-------------------|
| SubscriberRequestQuestion | Root, Message, Actor | Абонент задал вопрос | New |
| BotRepliedToUser | Message, Actor | Бот ответил | Active |
| OperatorDequeueRequest | Actor | Оператор взял обращение | Active |
| OperatorRepliedToMessage | Message, Actor | Оператор ответил | InProgress |
| OperatorClarifiedQuestion | Message, Actor | Оператор уточнил вопрос | InProgress |
| SubscriberGaveFeedback | Feedback | Абонент оценил работу | InProgress |
| SubscriberDownloadMedia | Root, Message, Actor | Абонент загрузил медиа | Active / InProgress |
| OperatorDownloadMedia | Message, Actor | Оператор загрузил медиа | InProgress |
| SessionEndingByTrigger | Message | Закрытие сессии | Active / InProgress |

## 7. Created Events

| Событие | Изменённые VO | Когда |
|---------|--------------|-------|
| ChatSessionCreated | Root, Message, Actor | После SubscriberRequestQuestion |
| BotReplied | Message, Actor | После BotRepliedToUser |
| RequestDequeued | Actor | После OperatorDequeueRequest |
| OperatorReplied | Message, Actor | После OperatorRepliedToMessage |
| QuestionClarified | Message, Actor | После OperatorClarifiedQuestion |
| FeedbackReceived | Feedback | После SubscriberGaveFeedback |
| MediaUploaded | Root, Message, Actor | После SubscriberDownloadMedia / OperatorDownloadMedia |
| SessionClosed | Message | После SessionEndingByTrigger |

## 8. Throughput

- **Ожидаемая частота:** от 40 до 3000+ tps в пике
- **Конкурентность:** низкая на агрегат (одна сессия ≈ один абонент + один оператор)
- **Один клиент — один агрегат:** Да
- **Профиль:** Ceqrs при нагрузке > 1000 tps, BudgetCqrs при < 100 tps

## 9. Size

- **Событий за жизнь:** 3–30 (короткие сессии)
- **Время жизни:** минуты — часы
- **Снапшоты:** не нужны (короткий lifecycle, мало событий)
- **Стратегия Event Store:** FullEventSourcing
