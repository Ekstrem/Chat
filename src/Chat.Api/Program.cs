using Autofac;
using Autofac.Extensions.DependencyInjection;
using Chat.Application.Commands;
using Chat.Domain;
using Chat.DomainServices;
using Chat.InternalContracts;
using Chat.Storage;
using Chat.Storage.ReadModel;
using DigiTFactory.Libraries.EventBus.InMemory.Extensions;
using DigiTFactory.Libraries.EventBus.Kafka.Extensions;
using DigiTFactory.Libraries.EventBus.Postgres.Extensions;
using DigiTFactory.Libraries.ReadRepository.Postgres.Extensions;
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

// EF Core + PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("ChatDb");
builder.Services.AddDbContext<CommandDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsql => npgsql.MigrationsAssembly("Chat.Storage")));

// Repository и QueryRepository
builder.Services.AddScoped<IQueryRepository<DomainEventEventEntry>, QueryRepository<DomainEventEventEntry>>();
builder.Services.AddScoped<IRepository, Repository>();

// ======================================================
// EventBus — выбор реализации через appsettings.json
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
// Read Store (проекции) — PostgreSQL
// ======================================================
var readDbConnectionString = builder.Configuration.GetConnectionString("ChatReadDb")
    ?? connectionString!;

builder.Services.AddReadStorePostgres<IChat, ChatReadModel, ReadDbContext>(
    readDbConnectionString,
    options => options.SchemaName = "ReadModel");

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
