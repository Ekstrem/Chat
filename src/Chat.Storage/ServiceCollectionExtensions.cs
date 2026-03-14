using System;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.DomainServices;
using Chat.Storage.Projections;
using DigiTFactory.Libraries.CommandRepository.Postgres.Configuration;
using DigiTFactory.Libraries.CommandRepository.Postgres.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatStorage(
            this IServiceCollection services,
            string connectionString,
            Action<EventStoreOptions>? configureOptions = null)
        {
            // Event Store из библиотеки (стратегия задаётся через configureOptions)
            services.AddEventStorePostgres<IChat, IChatAnemicModel>(connectionString, configureOptions);

            // Chat-специфичный DbContext для Read Models
            services.AddDbContext<CommandDbContext>(options =>
                options.UseNpgsql(connectionString, b =>
                    b.MigrationsAssembly("Chat.Storage")));

            // Chat-специфичные сервисы
            services.AddScoped<ChatProjectionService>();
            services.AddScoped<IRepository, Repository>();

            return services;
        }
    }
}
