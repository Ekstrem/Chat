# Руководство по реализации микросервиса

> Описание архитектурных принципов, слоёв, доменного моделирования и конфигурирования
> микросервисов на базе **DigiTFactory.Libraries** (Hive)

---

## 1. Архитектура слоёв

Микросервис строится по принципу **гексагональной (портовой) архитектуры** с чётким разделением на слои.
Зависимости направлены **строго внутрь** — внешние слои знают о внутренних, но не наоборот.

```
┌──────────────────────────────────────────────────────────┐
│                        Api                               │
│   Controllers, Program.cs, appsettings.json              │
│   Точка входа: DI-конфигурация, HTTP-маршруты            │
├──────────────────────────────────────────────────────────┤
│                    Application                           │
│   Commands, Queries, Handlers (MediatR)                  │
│   Оркестрация use-case-ов, CQRS-разделение               │
├──────────────────────────────────────────────────────────┤
│                   DomainServices                         │
│   AggregateProvider, BusAdapter, Notifier                │
│   Координация агрегатов, публикация событий               │
├──────────────────────────────────────────────────────────┤
│                      Domain                              │
│   Агрегаты, ValueObject-ы, спецификации/валидаторы       │
│   Чистая бизнес-логика без инфраструктурных зависимостей │
├──────────────────────────────────────────────────────────┤
│                     Storage                              │
│   DbContext, Repository, Mapping, Migrations             │
│   Адаптеры к БД (PostgreSQL / MongoDB)                   │
├──────────────────────────────────────────────────────────┤
│                 InternalContracts                         │
│   DTO, IQueryRepository, внутренние контракты            │
└──────────────────────────────────────────────────────────┘
```

### Правила зависимостей

| Слой | Может зависеть от | Не может зависеть от |
|------|-------------------|---------------------|
| **Domain** | SeedWorks | Всех остальных слоёв |
| **DomainServices** | Domain, SeedWorks, InternalContracts | Api, Application, Storage |
| **Application** | Domain, DomainServices, InternalContracts | Api, Storage |
| **Storage** | Domain, InternalContracts, SeedWorks | Api, Application, DomainServices |
| **Api** | Все слои (точка композиции) | — |

### Принцип работы слоя Domain

Слой Domain **не имеет инфраструктурных зависимостей** — только `SeedWorks`.
Здесь описываются:
- Bounded Context (маркерный интерфейс `IBoundedContext`)
- Агрегат (`IAggregate<TBC>`) и его анемичная модель (`IAnemicModel<TBC>`)
- Value Object-ы (`IValueObject`) — неизменяемые объекты-значения
- Aggregate Root (`IAggregateRoot<TBC>`) — корневая сущность агрегата
- Валидаторы/спецификации — бизнес-правила, проверяемые перед выполнением команд

### Принцип работы слоя DomainServices

Слой DomainServices координирует взаимодействие между доменом и инфраструктурой через абстракции:
- **AggregateProvider** — загружает агрегаты из репозитория, оборачивает в Notifier
- **BusAdapter** — реализует `IObserver<AggregateResult>`, публикует доменные события в `IEventBus`
- **Notifier** — декоратор агрегата, уведомляющий подписчиков о результатах бизнес-операций
- **RegisterDependencies** — модуль Autofac для регистрации доменных сервисов

### Принцип работы слоя Application

Слой Application реализует паттерн **CQRS** через MediatR:
- **Commands** — объекты-намерения (`IRequest<TResult>`), изменяющие состояние
- **Queries** — запросы на чтение, возвращающие проекции или анемичные модели
- **Handlers** — обработчики, которые загружают агрегат, вызывают бизнес-метод, сохраняют результат

---

## 2. Доменная модель на основе Aggregate Design Canvas

### Процесс проектирования

```
EventStorming (Big Picture)
        │
        ▼
Bounded Context Canvas          ← стратегический уровень
  (1 файл на контекст)            описывает ЧТО делает контекст
        │
        ▼
Aggregate Design Canvas         ← тактический уровень
  (1 файл на агрегат)             описывает КАК устроен агрегат внутри
        │
        ▼
Код / реализация
```

### 9 секций Aggregate Design Canvas

| # | Секция | Что описывает | Как влияет на код |
|---|--------|---------------|-------------------|
| 1 | **Name** | Название агрегата + lifecycle | Имя `IBoundedContext`, имя класса Aggregate |
| 2 | **Description** | Назначение и границы | Определяет набор Value Object-ов внутри агрегата |
| 3 | **State transitions** | Состояния и переходы | Логика бизнес-методов агрегата, валидаторы переходов |
| 4 | **Enforced invariants** | Гарантированные бизнес-правила | Реализуются как `IBusinessOperationValidator` |
| 5 | **Corrective policies** | Компенсирующие действия | Saga / Process Manager, обработчики событий |
| 6 | **Handled commands** | Входные команды | Методы агрегата, `IRequest<>` в Application |
| 7 | **Created events** | Порождаемые события | `IDomainEvent<TBC>`, публикация через `IEventBus` |
| 8 | **Throughput** | Частота и конкурентность | Решение о размере агрегата, стратегия event store |
| 9 | **Size** | Объём событий, время жизни | Необходимость снапшотов, стратегия хранения |

### Реализация в коде

#### Шаг 1. Bounded Context

```csharp
// Domain/IMyContext.cs
public interface IMyContext : IBoundedContext { }

// Domain/MyContextDescription.cs
public class MyContextDescription : IMyContext
{
    public string Name => "MyContext";
    public int MicroserviceVersion => 1;
}
```

#### Шаг 2. Aggregate Root и Value Object-ы (из секций 1-2 канваса)

Каждый Value Object — **неизменяемый** класс с фабричным методом `CreateInstance`:

```csharp
// Domain/Abstraction/IMyRoot.cs
public interface IMyRoot : IAggregateRoot<IMyContext>
{
    string Name { get; }
    Guid OwnerId { get; }
}

// Domain/Implementation/MyRoot.cs
internal sealed class MyRoot : IMyRoot
{
    public string Name { get; private set; }
    public Guid OwnerId { get; private set; }

    // Фабрика — единственный способ создания
    public static IMyRoot CreateInstance(string name, Guid ownerId) =>
        new MyRoot { Name = name, OwnerId = ownerId };
}
```

#### Шаг 3. Анемичная модель (композиция Value Object-ов)

```csharp
// Domain/Abstraction/IMyAnemicModel.cs
public interface IMyAnemicModel : IAnemicModel<IMyContext>
{
    IMyRoot Root { get; }
    IMyDetails Details { get; }
}

// Domain/Implementation/AnemicModel.cs
internal sealed class AnemicModel : AnemicModel<IMyContext>, IMyAnemicModel
{
    public IMyRoot Root { get; internal set; }
    public IMyDetails Details { get; internal set; }
}
```

#### Шаг 4. Агрегат с бизнес-методами (из секций 3, 6, 7 канваса)

Каждый метод агрегата соответствует **Handled Command** из канваса
и возвращает `AggregateResult`, содержащий доменное событие:

```csharp
// Domain/Implementation/Aggregate.cs
internal sealed class Aggregate : Aggregate<IMyContext>, IMyAnemicModel
{
    private AnemicModel _model = new();

    public AggregateResult<IMyContext, IMyAnemicModel> CreateSomething(
        IMyRoot root,
        IMyDetails details,
        IBusinessOperationValidator<IMyContext, IMyAnemicModel> validator)
    {
        return new BusinessOperationData<IMyContext, IMyAnemicModel>(this, _model, Command)
            .Validate(validator)
            .PipeTo(data =>
            {
                _model.Root = root;
                _model.Details = details;
                return data;
            })
            .Do(data => data.SetValueObjects(this))
            .ToResult();
    }
}
```

#### Шаг 5. Валидаторы (из секции 4 — Enforced Invariants)

```csharp
// Domain/Specifications/IsNewEntityValidator.cs
internal sealed class IsNewEntityValidator
    : IBusinessOperationValidator<IMyContext, IMyAnemicModel>
{
    public DomainOperationResultEnum Validate(IMyAnemicModel model)
    {
        if (model is not DefaultAnemicModel<IMyContext>)
            return DomainOperationResultEnum.Exception; // "Entity already exists"
        return DomainOperationResultEnum.Success;
    }
}
```

### Баланс границ агрегата (секции 8-9)

| Параметр | Малый агрегат | Большой агрегат |
|----------|--------------|-----------------|
| Инварианты | Меньше, eventual consistency | Больше, строгая консистентность |
| Конфликты | Мало | Много при высокой нагрузке |
| Масштабируемость | Лучше | Хуже |
| Сложность | Saga/Process Manager | Логика внутри агрегата |

**Эвристики:**
- Один клиент — один агрегат → можно сделать крупнее
- Много клиентов — один агрегат → дробить мельче
- Данные меняются вместе → один агрегат
- Если сомневаетесь — начните с маленького

---

## 3. Модель данных: CQRS / CEQRS

### Архитектура разделения хранилищ

В нашей реализации применяется паттерн **CEQRS** (Command Event Query Responsibility Segregation):

```
                      ┌─────────────────┐
     Команда ────────▶│   Application   │
                      │   (Command)     │
                      └────────┬────────┘
                               │
                      ┌────────▼────────┐
                      │  DomainServices  │
                      │  AggregateProvider│
                      │  + BusAdapter    │
                      └───┬─────────┬───┘
                          │         │
              ┌───────────▼───┐ ┌───▼───────────┐
              │  Command DB   │ │   Event Bus   │
              │  (Event Store)│ │  (IEventBus)  │
              │  PostgreSQL / │ │  InMemory /   │
              │  MongoDB      │ │  Kafka /      │
              └───────────────┘ │  Postgres     │
                                └───────┬───────┘
                                        │
                              ┌─────────▼─────────┐
                              │    Read Model DB   │
                              │    (Projections)   │
                              │    Отдельное       │
                              │    хранилище       │
                              └─────────┬──────────┘
                                        │
                              ┌─────────▼─────────┐
              Запрос ────────▶│    Application     │
                              │    (Query)         │
                              └────────────────────┘
```

### Command Side (запись)

**Event Store** — хранилище доменных событий. Является **источником истины** для состояния агрегата.

Структура записи в Event Store:

| Поле | Тип | Описание |
|------|-----|----------|
| `Id` | UUID | Идентификатор агрегата |
| `Version` | BIGINT | Версия (unix ms), часть составного ключа |
| `CorrelationToken` | UUID | Токен корреляции для трассировки |
| `CommandName` | TEXT | Имя выполненной команды |
| `SubjectName` | TEXT | Имя субъекта (агрегата) |
| `ChangedValueObjectsJson` | JSONB | Изменённые Value Object-ы (сериализованные) |
| `BoundedContext` | TEXT | Ограниченный контекст |
| `Result` | TEXT | Результат операции |
| `Reason` | TEXT | Причина (при ошибке) |
| `CreatedAt` | TIMESTAMPTZ | Время создания |

**Стратегии хранения** (настраиваются через `EventStoreOptions`):

| Стратегия | Описание | Когда применять |
|-----------|----------|-----------------|
| `FullEventSourcing` | Все события хранятся, агрегат восстанавливается из полного потока | По умолчанию. Полный audit trail |
| `SnapshotAfterN` | Снапшот каждые N событий + события после снапшота | Долгоживущие агрегаты с большим числом событий |
| `StateOnly` | Только текущее состояние, без истории | Простые агрегаты, где история не нужна |

### Query Side (чтение)

Read-модель — **материализованное представление** (проекция), денормализованная сущность,
построенная из доменных событий. Это **отдельное хранилище**, оптимизированное под запросы на чтение.

> **Важно:** Command DB и Read DB — это **разные базы данных** (или как минимум разные схемы).
> Write-сторона оптимизирована для консистентной записи событий.
> Read-сторона оптимизирована для быстрых запросов (денормализованные таблицы, индексы под конкретные use-case-ы).

Варианты Read Store:

| Хранилище | Библиотека | Когда использовать |
|-----------|-----------|-------------------|
| PostgreSQL | `ReadRepository.Postgres` | SQL-запросы, JOIN-ы, транзакционность |
| Redis | `ReadRepository.Redis` | Кэш, key-value, TTL, субмиллисекундные ответы |
| ScyllaDB | `ReadRepository.Scylla` | Высокая нагрузка, горизонтальное масштабирование |

### Event Bus — связующее звено

Доменные события, порождённые агрегатом, публикуются через `IEventBus`.
Потребители (проекции Read-модели, другие микросервисы) подписываются на эти события.

```
Aggregate ──▶ BusAdapter (IObserver) ──▶ IEventBus.Publish() ──▶ Projection Handler
                                                                      │
                                                              IReadModelStore.UpsertAsync()
                                                                      │
                                                                      ▼
                                                              Read DB (Postgres / Redis / ScyllaDB)
                                                                      │
                                                  Query ──▶ IReadRepository.GetByIdAsync() ──▶ Результат
```

---

## 4. Конфигурирование

### Структура appsettings.json

```jsonc
{
  // === Строки подключения ===
  "ConnectionStrings": {
    "CommandDb": "Host=localhost;Database=myservice_commands;...",
    "ReadDb": "Host=localhost;Database=myservice_read;..."
  },

  // === Выбор стратегии Event Bus ===
  "EventBus": {
    "Strategy": "InMemory",  // "InMemory" | "Kafka" | "Postgres"

    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "TopicPrefix": "my-events",
      "GroupId": "my-group"
    },

    "Postgres": {
      "ConnectionString": "Host=localhost;Database=myservice_events;...",
      "TableName": "domain_events_outbox",
      "PollingInterval": 5,
      "BatchSize": 100
    }
  },

  // === Внешние брокеры (при необходимости) ===
  "RabbitMq": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Регистрация в Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// IoC-контейнер (Autofac)
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new RegisterDependencies());
});

// MediatR для CQRS
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MyCommandHandler).Assembly));

// Command DB (Event Store) — выбор одного из вариантов:
builder.Services.AddEventStorePostgres<IMyContext, IMyAnemicModel>(
    connectionString,
    options => options.Strategy = EventStoreStrategy.FullEventSourcing);
// ИЛИ
builder.Services.AddEventStoreMongo<IMyContext, IMyAnemicModel>(options =>
{
    options.ConnectionString = "mongodb://localhost:27017";
    options.DatabaseName = "MyService";
});

// Event Bus — выбор реализации на основе конфигурации:
var busStrategy = builder.Configuration["EventBus:Strategy"];
switch (busStrategy)
{
    case "Kafka":
        builder.Services.AddEventBusKafka(options =>
            builder.Configuration.GetSection("EventBus:Kafka").Bind(options));
        break;
    case "Postgres":
        builder.Services.AddEventBusPostgres(options =>
            builder.Configuration.GetSection("EventBus:Postgres").Bind(options));
        break;
    default:
        builder.Services.AddEventBusInMemory();
        break;
}

// Read Store (проекции) — PostgreSQL:
var readConnStr = builder.Configuration.GetConnectionString("ReadDb");
builder.Services.AddReadStorePostgres<IMyContext, MyReadModel, MyReadDbContext>(
    readConnStr,
    options => options.SchemaName = "ReadModel");
```

### Замечания

- **InMemory EventBus** — только для разработки и тестов. События теряются при перезапуске процесса.
- **Kafka EventBus** — для продакшена с высокой нагрузкой. Требует работающий кластер Kafka. Топики создаются автоматически: `{TopicPrefix}.{boundedContext}`.
- **Postgres EventBus** — паттерн Outbox. Таблица создаётся автоматически при `AutoCreateTable = true`. Использует `FOR UPDATE SKIP LOCKED` для конкурентной обработки.
- **Connection strings** — никогда не хранить в appsettings.json в продакшене. Использовать переменные окружения или secret manager.
- **Autofac** обязателен — модуль `RegisterDependencies` использует Autofac `Module` для регистрации доменных сервисов.

---

## 5. Библиотеки DigiTFactory

### Обзор пакетов

```
DigiTFactory.Libraries (Hive)
├── SeedWorks                     ← ядро: интерфейсы, базовые классы, монады
├── CommandRepository.Postgres    ← Event Store на PostgreSQL (EF Core)
├── CommandRepository.Mongo       ← Event Store на MongoDB
├── EventBus.InMemory             ← шина событий для тестов
├── EventBus.Kafka                ← шина событий для продакшена (Kafka)
├── EventBus.Postgres             ← шина событий через Outbox-таблицу (Postgres)
├── ReadRepository.Postgres       ← Read Store на PostgreSQL (EF Core)
├── ReadRepository.Redis          ← Read Store на Redis (StackExchange.Redis)
└── ReadRepository.Scylla         ← Read Store на ScyllaDB/Cassandra
```

### SeedWorks — ядро фреймворка

Ключевые абстракции:

| Интерфейс | Назначение |
|-----------|-----------|
| `IBoundedContext` | Маркер ограниченного контекста |
| `IAggregate<TBC>` | Агрегат с бизнес-логикой |
| `IAnemicModel<TBC>` | Анемичная модель — контейнер состояния |
| `IAggregateRoot<TBC>` | Корневая сущность (Value Object) |
| `IValueObject` | Маркер объекта-значения |
| `ICommandToAggregate` | Метаданные команды (Version, CorrelationToken, Name) |
| `IDomainEvent<TBC>` | Доменное событие (Command + ChangedValueObjects + Result) |
| `IEventBus` | `IEventBusProducer` + `IEventBusConsumer` |
| `IAnemicModelRepository<TBC, TAM>` | Репозиторий анемичных моделей |
| `IReadModel<TBC>` | Маркер Read-модели (проекции) |
| `IReadRepository<TBC, TRM>` | Репозиторий чтения проекций |
| `IReadModelStore<TBC, TRM>` | Хранилище записи проекций (upsert/delete) |
| `IBusinessOperationValidator<TBC, TAM>` | Валидатор бизнес-правил |

Утилиты:

| Элемент | Назначение |
|---------|-----------|
| `ComplexKey` | Составной ключ: `Id + Version + CorrelationToken` |
| `CommandToAggregate.Commit(...)` | Фабрика команд |
| `AggregateResult` | Результат бизнес-операции (Success / WithWarnings / Exception) |
| `Result<S,F>` | Монада результата с `Match` |
| `.PipeTo()`, `.Do()` | Функциональные расширения для цепочек |

### CommandRepository — хранилище команд (Event Store)

**Postgres** (`AddEventStorePostgres`):
```csharp
services.AddEventStorePostgres<IMyContext, IMyAnemicModel>(
    connectionString,
    options =>
    {
        options.Strategy = EventStoreStrategy.FullEventSourcing;
        options.SchemaName = "Commands";
        options.SnapshotInterval = 10;  // для стратегии SnapshotAfterN
    });
```

**MongoDB** (`AddEventStoreMongo`):
```csharp
services.AddEventStoreMongo<IMyContext, IMyAnemicModel>(options =>
{
    options.ConnectionString = "mongodb://localhost:27017";
    options.DatabaseName = "MyService";
    options.Strategy = EventStoreStrategy.FullEventSourcing;
    options.CollectionPrefix = "MyContext_";
});
```

Оба варианта регистрируют `IAnemicModelRepository<TBC, TAM>` — основной репозиторий для загрузки и сохранения агрегатов.

### EventBus — шина доменных событий

| Реализация | Метод регистрации | Когда использовать |
|-----------|-------------------|--------------------|
| InMemory | `AddEventBusInMemory()` | Разработка, тесты, один процесс |
| Kafka | `AddEventBusKafka(options)` | Продакшен, высокая нагрузка, несколько потребителей |
| Postgres | `AddEventBusPostgres(options)` | Outbox-паттерн, транзакционная гарантия доставки |

Каждый пакет регистрирует:
- `IEventBus` — объединённый интерфейс
- `IEventBusProducer` — для публикации событий
- `IEventBusConsumer` — для подписки на события
- `IHostedService` (Kafka, Postgres) — фоновый consumer

### ReadRepository — хранилище проекций (Read Store)

Read-модель — **материализованное представление**, денормализованная сущность, проецируемая из доменных событий. Каждая Read-модель реализует `IReadModel<TBC>`.

Поток данных:
```
Aggregate → DomainEvent → IEventBus → Projection Handler → IReadModelStore.UpsertAsync() → Read DB
Query     → IReadRepository.GetByIdAsync() → Read DB → Результат
```

| Реализация | Метод регистрации | Когда использовать |
|-----------|-------------------|--------------------|
| Postgres | `AddReadStorePostgres<TBC, TRM, TDbContext>(connStr)` | Реляционные проекции, SQL-запросы, транзакционность |
| Redis | `AddReadStoreRedis<TBC, TRM>(options)` | Кэширование, быстрый key-value доступ, TTL |
| ScyllaDB | `AddReadStoreScylla<TBC, TRM>(options)` | Высокая нагрузка на чтение, горизонтальное масштабирование |

**Postgres** (`AddReadStorePostgres`):
```csharp
services.AddReadStorePostgres<IMyContext, MyReadModel, MyReadDbContext>(
    readConnectionString,
    options => options.SchemaName = "ReadModel");
```

**Redis** (`AddReadStoreRedis`):
```csharp
services.AddReadStoreRedis<IMyContext, MyReadModel>(options =>
{
    options.ConnectionString = "localhost:6379";
    options.KeyPrefix = "myservice:read:";
    options.DefaultTtl = TimeSpan.FromMinutes(30);
});
```

**ScyllaDB** (`AddReadStoreScylla`):
```csharp
services.AddReadStoreScylla<IMyContext, MyReadModel>(options =>
{
    options.ContactPoints = new[] { "scylla-node1", "scylla-node2" };
    options.Keyspace = "myservice_read";
    options.TableName = "projections";
    options.ReplicationFactor = 3;
});
```

Каждый пакет регистрирует:
- `IReadRepository<TBC, TRM>` — чтение проекций (`GetByIdAsync`, `GetAllAsync`, `CountAsync`)
- `IReadModelStore<TBC, TRM>` — запись проекций (`UpsertAsync`, `DeleteAsync`)

### Версионирование пакетов

Текущие версии:

| Пакет | Версия |
|-------|--------|
| `DigiTFactory.Libraries.SeedWorks` | 0.4.0 |
| `DigiTFactory.Libraries.CommandRepository.Postgres` | 0.1.0 |
| `DigiTFactory.Libraries.CommandRepository.Mongo` | 0.1.0 |
| `DigiTFactory.Libraries.EventBus.InMemory` | 0.1.0 |
| `DigiTFactory.Libraries.EventBus.Kafka` | 0.1.0 |
| `DigiTFactory.Libraries.EventBus.Postgres` | 0.1.0 |
| `DigiTFactory.Libraries.ReadRepository.Postgres` | 0.1.0 |
| `DigiTFactory.Libraries.ReadRepository.Redis` | 0.1.0 |
| `DigiTFactory.Libraries.ReadRepository.Scylla` | 0.1.0 |

---

## Чек-лист создания нового микросервиса

1. **Стратегический дизайн** — заполнить Bounded Context Canvas
2. **Тактический дизайн** — заполнить Aggregate Design Canvas для каждого агрегата
3. **Создать проекты** — Api, Application, Domain, DomainServices, Storage, InternalContracts
4. **Domain** — определить `IBoundedContext`, Value Object-ы, Aggregate Root, Aggregate, AnemicModel, валидаторы
5. **DomainServices** — реализовать AggregateProvider, BusAdapter, Notifier, RegisterDependencies
6. **Application** — создать Commands и Queries через MediatR
7. **Storage (Command)** — настроить CommandDbContext, Repository, маппинг событий
8. **Storage (Read)** — определить ReadModel (`IReadModel<TBC>`), ReadDbContext, маппинг проекций
9. **Api** — сконфигурировать DI (Autofac + MediatR + EventStore + EventBus + ReadStore), создать контроллеры
10. **Тесты** — написать unit-тесты на бизнес-логику агрегатов
11. **CI/CD** — настроить GitHub Actions для сборки и деплоя
