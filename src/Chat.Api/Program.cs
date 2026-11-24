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

// Event Store: полный replay выполняет библиотека через доменный IEventStreamRehydrator.
builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseNpgsql(commandDb, npgsql => npgsql
        .MigrationsAssembly("Chat.Storage")
        .EnableRetryOnFailure()));

builder.Services.AddScoped<IEventStreamRehydrator<IChat, IChatAnemicModel>, MessageEventStreamRehydrator>();
builder.Services.AddEventStorePostgres<IChat, IChatAnemicModel>(commandDb, options =>
{
    options.Strategy = EventStoreStrategy.FullEventSourcing;
    options.SchemaName = eventStoreSchema;
});

builder.Services.AddEventBusInMemory();

// ReadRepository.Postgres регистрирует IReadRepository, IQueryableReadRepository и IReadModelStore.
builder.Services.AddReadStorePostgres<IChat, MessageReadModel, ReadDbContext>(readDb, options => options.SchemaName = readStoreSchema);
builder.Services.AddReadStorePostgres<IChat, InboxRowReadModel, ReadDbContext>(readDb, options => options.SchemaName = readStoreSchema);
builder.Services.AddReadStorePostgres<IChat, LastReadReadModel, ReadDbContext>(readDb, options => options.SchemaName = readStoreSchema);

// Durable-запись и проекции — независимые подписчики шины.
builder.Services.AddSingleton<MessageEventStoreHandler>();
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
eventBus.Subscribe<IChat>(app.Services.GetRequiredService<MessageEventStoreHandler>());
eventBus.Subscribe<IChat>(app.Services.GetRequiredService<MessageEventHandler>());

app.UseSwagger();
app.UseSwaggerUI(configuration => configuration.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat API v1"));
app.MapControllers();
app.MapHealthChecks("/hc");
app.Run();

/// <summary>Точка входа, доступная тестам уровня 3.</summary>
public partial class Program { }
