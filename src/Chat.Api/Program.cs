using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Chat.Application;
using Chat.Application.Behaviors;
using Chat.DomainServices;
using Chat.DomainServices.Consumers;
using Chat.Storage;
using Chat.Storage.Postgres;
using DigiTFactory.Libraries.CommandRepository.Postgres;
using DigiTFactory.Libraries.CommandRepository.Postgres.Configuration;
// using Chat.Storage.Mongo;
// using DigiTFactory.Libraries.CommandRepository.Mongo.Configuration;
// using MongoStrategy = DigiTFactory.Libraries.CommandRepository.Mongo.Configuration.EventStoreStrategy;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================================================
// Выбор СУБД для Event Store.
// Раскомментируйте ОДНУ из секций ниже.
// ======================================================

// --- PostgreSQL ---
builder.Services.AddChatPostgresStorage(
    builder.Configuration.GetConnectionString("CommandDb")!,
    options =>
    {
        var section = builder.Configuration.GetSection("EventStore");
        if (Enum.TryParse<EventStoreStrategy>(section["Strategy"], out var strategy))
            options.Strategy = strategy;
        if (int.TryParse(section["SnapshotInterval"], out var interval))
            options.SnapshotInterval = interval;
        options.SchemaName = section["SchemaName"] ?? "Commands";
    });

// --- MongoDB ---
// builder.Services.AddChatMongoStorage(
//     builder.Configuration.GetConnectionString("CommandDb")!,
//     options =>
//     {
//         var mongoSection = builder.Configuration.GetSection("MongoEventStore");
//         options.ConnectionString = mongoSection["ConnectionString"] ?? "mongodb://localhost:27017";
//         options.DatabaseName = mongoSection["DatabaseName"] ?? "ChatEventStore";
//         if (Enum.TryParse<MongoStrategy>(mongoSection["Strategy"], out var strategy))
//             options.Strategy = strategy;
//         if (int.TryParse(mongoSection["SnapshotInterval"], out var interval))
//             options.SnapshotInterval = interval;
//     });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ChatOperationResult).Assembly));

// Register pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DomainEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rabbitConfig["Host"] ?? "localhost", h =>
        {
            h.Username(rabbitConfig["Username"] ?? "guest");
            h.Password(rabbitConfig["Password"] ?? "guest");
        });

        cfg.ReceiveEndpoint("chat-events", e =>
        {
            e.ConfigureConsumer<DomainEventConsumer>(context);
        });
    });
});

// Autofac for DomainServices
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    containerBuilder.RegisterModule<RegisterDependencies>());

// CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Auto-create DB in dev
    using var scope = app.Services.CreateScope();
    var eventStoreDb = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
    eventStoreDb.Database.EnsureCreated();
    var readModelDb = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
    readModelDb.Database.EnsureCreated();
}

app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
