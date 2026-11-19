using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.InternalContracts;
using Chat.Storage;
using DigiTFactory.Libraries.ReadRepository.Postgres;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.IntegrationTests;

/// <summary>
/// Р СћР ВµРЎРѓРЎвЂљР С•Р Р†РЎвЂ№Р в„– РЎРѓРЎвЂљР ВµР Р…Р Т‘ РЎС“РЎР‚Р С•Р Р†Р Р…РЎРЏ 2: РЎР‚Р ВµР В°Р В»РЎРЉР Р…РЎвЂ№Р Вµ Р С—РЎР‚Р С•Р ВµР С”РЎвЂ Р С‘Р С‘ Р С‘ Read Store (EF InMemory), Р В±Р ВµР В· Postgres.
/// </summary>
public sealed class ProjectionHarness : IDisposable
{
    private static readonly InMemoryDatabaseRoot DatabaseRoot = new();
    private static readonly string DatabaseName = $"chat-{Guid.NewGuid():N}";
    private readonly ServiceProvider _provider;

    public ProjectionHarness()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Warning);
            builder.AddProvider(new ConsoleLoggerProvider());
        });
        services.AddDbContext<ReadDbContext>(options => options.UseInMemoryDatabase(DatabaseName, DatabaseRoot));

        services.AddScoped<IReadModelStore<IChat, MessageReadModel>>(provider =>
            new PostgresReadModelStore<IChat, MessageReadModel>(provider.GetRequiredService<ReadDbContext>()));
        services.AddScoped<IReadRepository<IChat, MessageReadModel>>(provider =>
            new PostgresReadRepository<IChat, MessageReadModel>(provider.GetRequiredService<ReadDbContext>()));
        services.AddScoped<IReadModelStore<IChat, InboxRowReadModel>>(provider =>
            new PostgresReadModelStore<IChat, InboxRowReadModel>(provider.GetRequiredService<ReadDbContext>()));
        services.AddScoped<IReadRepository<IChat, InboxRowReadModel>>(provider =>
            new PostgresReadRepository<IChat, InboxRowReadModel>(provider.GetRequiredService<ReadDbContext>()));
        services.AddScoped<IReadModelStore<IChat, LastReadReadModel>>(provider =>
            new PostgresReadModelStore<IChat, LastReadReadModel>(provider.GetRequiredService<ReadDbContext>()));
        services.AddScoped<IReadRepository<IChat, LastReadReadModel>>(provider =>
            new PostgresReadRepository<IChat, LastReadReadModel>(provider.GetRequiredService<ReadDbContext>()));

        services.AddSingleton<MessageStateCache>();
        services.AddScoped<MessageStateRepository>();
        services.AddScoped<IAnemicModelRepository<IChat, IChatAnemicModel>>(provider =>
            provider.GetRequiredService<MessageStateRepository>());
        services.AddSingleton<MessageEventHandler>();

        _provider = services.BuildServiceProvider();
    }

    public MessageEventHandler Handler => _provider.GetRequiredService<MessageEventHandler>();

    public ReadDbContext ReadDb => _provider.GetRequiredService<ReadDbContext>();

    public MessageStateRepository StateRepository => _provider.GetRequiredService<MessageStateRepository>();

    public static IChatAggregate EmptyAggregate(Guid id)
        => Chat.Domain.Implementation.Aggregate.CreateInstance(Chat.Domain.Implementation.DefaultAnemicModel.Create(id));

    public void Dispose() => _provider.Dispose();
}

/// <summary>Логгер тестового стенда: пишет в консоль, чтобы видеть причины сбоев проекций.</summary>
internal sealed class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName);

    public void Dispose() { }

    private sealed class ConsoleLogger : ILogger
    {
        private readonly string _category;

        public ConsoleLogger(string category) => _category = category;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Console.WriteLine($"[{logLevel}] {_category}: {formatter(state, exception)} {exception}");
    }
}