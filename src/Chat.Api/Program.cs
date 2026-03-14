using Autofac;
using Autofac.Extensions.DependencyInjection;
using Chat.Application.Commands;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.InternalContracts;
using Chat.Storage;
using Chat.Storage.ReadModel;
using DigiTFactory.Libraries.CommandRepository.Mongo.Extensions;
using DigiTFactory.Libraries.EventBus.InMemory.Extensions;
using DigiTFactory.Libraries.EventBus.Kafka.Extensions;
using DigiTFactory.Libraries.EventBus.Postgres.Extensions;
using DigiTFactory.Libraries.ReadRepository.Postgres.Extensions;
using DigiTFactory.Libraries.ReadRepository.Redis.Extensions;
using DigiTFactory.Libraries.ReadRepository.Scylla.Extensions;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Autofac
var host = builder.Host;
host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<RegisterDependencies>();
});

// ======================================================
// Command Store — выбор реализации через appsettings
// ======================================================
var commandStoreProvider = builder.Configuration["CommandStore:Provider"] ?? "Postgres";

switch (commandStoreProvider)
{
    case "Mongo":
        builder.Services.AddEventStoreMongo<IChat, IChatAnemicModel>(options =>
        {
            var mongo = builder.Configuration.GetSection("CommandStore:Mongo");
            options.ConnectionString = mongo["ConnectionString"] ?? "mongodb://localhost:27017";
            options.DatabaseName = mongo["DatabaseName"] ?? "chat_commands";
        });
        break;

    case "Postgres":
    default:
        var connectionString = builder.Configuration.GetConnectionString("ChatDb");
        builder.Services.AddDbContext<CommandDbContext>(options =>
            options.UseNpgsql(connectionString,
                npgsql => npgsql.MigrationsAssembly("Chat.Storage")));

        builder.Services.AddScoped<IQueryRepository<DomainEventEventEntry>, QueryRepository<DomainEventEventEntry>>();
        builder.Services.AddScoped<IRepository, Repository>();
        break;
}

// ======================================================
// EventBus — выбор реализации через appsettings
// ======================================================
var eventBusSection = builder.Configuration.GetSection("EventBus");
var eventBusStrategy = eventBusSection["Strategy"] ?? "InMemory";

switch (eventBusStrategy)
{
    case "Kafka":
        builder.Services.AddEventBusKafka(options =>
        {
            var kafka = eventBusSection.GetSection("Kafka");
            options.BootstrapServers = kafka["BootstrapServers"] ?? "localhost:9092";
            options.TopicPrefix = kafka["TopicPrefix"] ?? "chat-events";
            options.GroupId = kafka["GroupId"] ?? "chat-group";
        });
        break;

    case "Postgres":
        builder.Services.AddEventBusPostgres(options =>
        {
            var pg = eventBusSection.GetSection("Postgres");
            options.ConnectionString = pg["ConnectionString"]
                ?? builder.Configuration.GetConnectionString("ChatDb")!;
            options.TableName = pg["TableName"] ?? "domain_events_outbox";
            if (int.TryParse(pg["PollingInterval"], out var interval))
                options.PollingInterval = TimeSpan.FromSeconds(interval);
            if (int.TryParse(pg["BatchSize"], out var batch))
                options.BatchSize = batch;
        });
        break;

    case "InMemory":
    default:
        builder.Services.AddEventBusInMemory();
        break;
}

// ======================================================
// Read Store (проекции) — выбор реализации через appsettings
// ======================================================
var readStoreProvider = builder.Configuration["ReadStore:Provider"] ?? "Postgres";

switch (readStoreProvider)
{
    case "Redis":
        builder.Services.AddReadStoreRedis<IChat, ChatReadModel>(options =>
        {
            var redis = builder.Configuration.GetSection("ReadStore:Redis");
            options.ConnectionString = redis["ConnectionString"] ?? "localhost:6379";
            options.KeyPrefix = redis["KeyPrefix"] ?? "chat:read:";
            if (int.TryParse(redis["DefaultTtlMinutes"], out var ttl))
                options.DefaultTtl = TimeSpan.FromMinutes(ttl);
        });
        break;

    case "Scylla":
        builder.Services.AddReadStoreScylla<IChat, ChatReadModel>(options =>
        {
            var scylla = builder.Configuration.GetSection("ReadStore:Scylla");
            options.ContactPoints = scylla.GetSection("ContactPoints").Get<string[]>()
                ?? new[] { "localhost" };
            options.Keyspace = scylla["Keyspace"] ?? "chat_read";
            options.TableName = scylla["TableName"] ?? "projections";
            if (int.TryParse(scylla["ReplicationFactor"], out var rf))
                options.ReplicationFactor = rf;
            if (bool.TryParse(scylla["AutoCreateSchema"], out var autoCreate))
                options.AutoCreateSchema = autoCreate;
        });
        break;

    case "Postgres":
    default:
        var readDbConnectionString = builder.Configuration.GetConnectionString("ChatReadDb");
        if (string.IsNullOrEmpty(readDbConnectionString))
            readDbConnectionString = builder.Configuration.GetConnectionString("ChatDb")!;

        var readSchemaName = builder.Configuration["ReadStore:SchemaName"] ?? "ReadModel";

        builder.Services.AddReadStorePostgres<IChat, ChatReadModel, ReadDbContext>(
            readDbConnectionString,
            options => options.SchemaName = readSchemaName);
        break;
}

// ======================================================
// Projection Handler + Rebuild Service
// ======================================================
builder.Services.AddScoped<ChatProjectionHandler>();
builder.Services.AddScoped<IRebuildService<IChat>, ChatRebuildService>();

// MassTransit + RabbitMQ
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqConfig["Host"] ?? "localhost", "/", h =>
        {
            h.Username(rabbitMqConfig["Username"] ?? "guest");
            h.Password(rabbitMqConfig["Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SubscriberRequestQuestionCommand).Assembly));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Chat Microservice API",
        Version = "v1",
        Description = "Reference DDD/CQRS/Event Sourcing microservice"
    });
});

var app = builder.Build();

// Подписать ProjectionHandler на EventBus
using (var scope = app.Services.CreateScope())
{
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    var projectionHandler = scope.ServiceProvider.GetRequiredService<ChatProjectionHandler>();
    eventBus.Subscribe<IChat>(projectionHandler);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
