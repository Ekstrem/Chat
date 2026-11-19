using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.InternalContracts;
using Chat.Storage;
using DigiTFactory.Libraries.CommandRepository.Postgres.Entities;
using DigiTFactory.Libraries.CommandRepository.Postgres.Repositories;
using DigiTFactory.Libraries.ReadRepository.Postgres;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Chat.ApiTests;

/// <summary>
/// РЎС‚РµРЅРґ СѓСЂРѕРІРЅСЏ 3: РїСЂРёР»РѕР¶РµРЅРёРµ С†РµР»РёРєРѕРј С‡РµСЂРµР· HTTP, РЅРѕ Read Store Рё СЃРѕСЃС‚РѕСЏРЅРёРµ Р°РіСЂРµРіР°С‚Р° вЂ”
/// РІ РїР°РјСЏС‚Рё (Р±РµР· Postgres).
/// </summary>
public sealed class ChatApiFactory : WebApplicationFactory<Program>
{
    private static readonly InMemoryDatabaseRoot InMemoryRoot = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ReadDbContext>();
            services.RemoveAll<DbContextOptions<ReadDbContext>>();
            services.AddDbContext<ReadDbContext>(options => options.UseInMemoryDatabase("chat-api-tests", InMemoryRoot));

            services.RemoveAll<IReadModelStore<IChat, MessageReadModel>>();
            services.RemoveAll<IReadModelStore<IChat, InboxRowReadModel>>();
            services.RemoveAll<IReadModelStore<IChat, LastReadReadModel>>();
            services.RemoveAll<IReadRepository<IChat, MessageReadModel>>();
            services.RemoveAll<IReadRepository<IChat, InboxRowReadModel>>();
            services.RemoveAll<IReadRepository<IChat, LastReadReadModel>>();
            services.RemoveAll<IQueryableReadRepository<IChat, MessageReadModel>>();
            services.RemoveAll<IQueryableReadRepository<IChat, InboxRowReadModel>>();
            services.RemoveAll<IQueryableReadRepository<IChat, LastReadReadModel>>();
            services.RemoveAll<IAnemicModelRepository<IChat, IChatAnemicModel>>();
            services.RemoveAll<IEventStoreRepository<IChat, IChatAnemicModel>>();
            services.AddScoped<IEventStoreRepository<IChat, IChatAnemicModel>, NullEventStoreRepository>();

            services.AddScoped<IReadModelStore<IChat, MessageReadModel>>(provider =>
                new PostgresReadModelStore<IChat, MessageReadModel>(provider.GetRequiredService<ReadDbContext>()));
            services.AddScoped<IReadModelStore<IChat, InboxRowReadModel>>(provider =>
                new PostgresReadModelStore<IChat, InboxRowReadModel>(provider.GetRequiredService<ReadDbContext>()));
            services.AddScoped<IReadModelStore<IChat, LastReadReadModel>>(provider =>
                new PostgresReadModelStore<IChat, LastReadReadModel>(provider.GetRequiredService<ReadDbContext>()));

            services.AddScoped<IReadRepository<IChat, MessageReadModel>>(provider =>
                new PostgresReadRepository<IChat, MessageReadModel>(provider.GetRequiredService<ReadDbContext>()));
            services.AddScoped<IReadRepository<IChat, InboxRowReadModel>>(provider =>
                new PostgresReadRepository<IChat, InboxRowReadModel>(provider.GetRequiredService<ReadDbContext>()));
            services.AddScoped<IReadRepository<IChat, LastReadReadModel>>(provider =>
                new PostgresReadRepository<IChat, LastReadReadModel>(provider.GetRequiredService<ReadDbContext>()));

            services.AddScoped<IQueryableReadRepository<IChat, MessageReadModel>>(provider =>
                new ReadModelQuery<MessageReadModel>(provider.GetRequiredService<ReadDbContext>()));
            services.AddScoped<IQueryableReadRepository<IChat, InboxRowReadModel>>(provider =>
                new ReadModelQuery<InboxRowReadModel>(provider.GetRequiredService<ReadDbContext>()));
            services.AddScoped<IQueryableReadRepository<IChat, LastReadReadModel>>(provider =>
                new ReadModelQuery<LastReadReadModel>(provider.GetRequiredService<ReadDbContext>()));

            services.AddScoped<MessageStateRepository>();
            services.AddScoped<IAnemicModelRepository<IChat, IChatAnemicModel>>(provider =>
                provider.GetRequiredService<MessageStateRepository>());
        });
    }
}
/// <summary>Event Store-заглушка тестов: события не сохраняются (Postgres в тестах нет).</summary>
internal sealed class NullEventStoreRepository : IEventStoreRepository<IChat, IChatAnemicModel>
{
    public Task<List<IChatAnemicModel>> GetById(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(new List<IChatAnemicModel>());

    public Task<IChatAnemicModel> GetByIdAndVersion(Guid id, long version, CancellationToken cancellationToken)
        => Task.FromResult<IChatAnemicModel>(null!);

    public Task<IChatAnemicModel> GetByCorrelationToken(Guid correlationToken, CancellationToken cancellationToken)
        => Task.FromResult<IChatAnemicModel>(null!);

    public Task SaveEventAsync(DomainEventEntry entry, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SaveSnapshotAsync(Guid id, long version, string aggregateJson, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<int> GetEventCountAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(0);
}