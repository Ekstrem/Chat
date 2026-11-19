using Autofac;
using Autofac.Extensions.DependencyInjection;
using Chat.Application.Commands;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.InternalContracts;
using Chat.Storage;
using DigiTFactory.Libraries.CommandRepository.Postgres;
using DigiTFactory.Libraries.CommandRepository.Postgres.Configuration;
using DigiTFactory.Libraries.CommandRepository.Postgres.Extensions;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using DigiTFactory.Libraries.EventBus.InMemory.Extensions;
using DigiTFactory.Libraries.ReadRepository.Postgres.Extensions;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container => container.RegisterModule<RegisterDependencies>());

var commandDb = builder.Configuration.GetConnectionString("CommandDb")
                ?? throw new InvalidOperationException("ConnectionStrings:CommandDb не задан");
var readDb = builder.Configuration.GetConnectionString("ReadDb")
             ?? throw new InvalidOperationException("ConnectionStrings:ReadDb не задан");

var eventStoreSchema = builder.Configuration["EventStore:SchemaName"] ?? "Commands";
var readStoreSchema = builder.Configuration["ReadStore:SchemaName"] ?? "ReadModel";

// Event Store: схемой управляют миграции этой сборки, стратегия — полный Event Sourcing.
builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseNpgsql(commandDb, npgsql => npgsql
        .MigrationsAssembly("Chat.Storage")
        .EnableRetryOnFailure()));

builder.Services.AddEventStorePostgres<IChat, IChatAnemicModel>(commandDb, options =>
{
    options.Strategy = EventStoreStrategy.FullEventSourcing;
    options.SchemaName = eventStoreSchema;
});

// Чтение Event Store: библиотечная реализация — заглушка, поэтому наследуем и переопределяем.
builder.Services.AddScoped<ChatEventStoreRepository>();
builder.Services.AddScoped<IEventStoreRepository<IChat, IChatAnemicModel>>(provider =>
    provider.GetRequiredService<ChatEventStoreRepository>());
builder.Services.AddScoped<IAnemicModelRepository<IChat, IChatAnemicModel>>(provider =>
    provider.GetRequiredService<ChatEventStoreRepository>());

builder.Services.AddEventBusInMemory();

builder.Services.AddReadStorePostgres<IChat, MessageReadModel, ReadDbContext>(readDb, options => options.SchemaName = readStoreSchema);
builder.Services.AddReadStorePostgres<IChat, InboxRowReadModel, ReadDbContext>(readDb, options => options.SchemaName = readStoreSchema);
builder.Services.AddReadStorePostgres<IChat, LastReadReadModel, ReadDbContext>(readDb, options => options.SchemaName = readStoreSchema);

// Библиотека регистрирует только IReadRepository; запросам нужен queryable-доступ к проекциям.
builder.Services.AddScoped<IQueryableReadRepository<IChat, MessageReadModel>>(provider =>
    new ReadModelQuery<MessageReadModel>(provider.GetRequiredService<ReadDbContext>()));
builder.Services.AddScoped<IQueryableReadRepository<IChat, InboxRowReadModel>>(provider =>
    new ReadModelQuery<InboxRowReadModel>(provider.GetRequiredService<ReadDbContext>()));
builder.Services.AddScoped<IQueryableReadRepository<IChat, LastReadReadModel>>(provider =>
    new ReadModelQuery<LastReadReadModel>(provider.GetRequiredService<ReadDbContext>()));

// Проекции: обработчик подписывается на шину.
builder.Services.AddSingleton<MessageEventHandler>();

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssemblyContaining<SendMessageCommand>());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Схема Command DB и Read Store создаётся миграциями сборки Chat.Storage.
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<EventStoreDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<ReadDbContext>().Database.Migrate();
}

var eventBus = app.Services.GetRequiredService<IEventBus>();
var projectionHandler = app.Services.GetRequiredService<MessageEventHandler>();
eventBus.Subscribe<IChat>(projectionHandler);

app.UseSwagger();
app.UseSwaggerUI(configuration => configuration.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat API v1"));
app.MapControllers();
app.MapHealthChecks("/hc");
app.Run();

/// <summary>Точка входа, доступная тестам уровня 3.</summary>
public partial class Program { }
