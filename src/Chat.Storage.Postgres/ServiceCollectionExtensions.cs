#nullable enable
using System;
using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Postgres.Configuration;
using DigiTFactory.Libraries.CommandRepository.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Storage.Postgres
{
    /// <summary>
    /// Регистрация Chat Storage с PostgreSQL Event Store.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Подключает PostgreSQL Event Store + CommandDbContext.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="connectionString">Строка подключения к PostgreSQL (используется и для Event Store, и для CommandDb).</param>
        /// <param name="configureOptions">Настройки Event Store (стратегия, интервал снапшотов, схема).</param>
        public static IServiceCollection AddChatPostgresStorage(
            this IServiceCollection services,
            string connectionString,
            Action<EventStoreOptions>? configureOptions = null)
        {
            // Event Store из библиотеки CommandRepository.Postgres
            services.AddEventStorePostgres<IChat, IChatAnemicModel>(connectionString, configureOptions);

            // Общие Chat-компоненты (CommandDbContext, Projections, IRepository)
            services.AddChatCommandDb(connectionString);

            return services;
        }
    }
}
