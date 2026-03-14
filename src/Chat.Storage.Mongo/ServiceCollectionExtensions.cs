#nullable enable
using System;
using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.CommandRepository.Mongo.Configuration;
using DigiTFactory.Libraries.CommandRepository.Mongo.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Storage.Mongo
{
    /// <summary>
    /// Регистрация Chat Storage с MongoDB Event Store.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Подключает MongoDB Event Store + CommandDbContext (PostgreSQL).
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="connectionString">Строка подключения к PostgreSQL для CommandDb.</param>
        /// <param name="configureOptions">Настройки MongoDB Event Store.</param>
        public static IServiceCollection AddChatMongoStorage(
            this IServiceCollection services,
            string connectionString,
            Action<MongoEventStoreOptions> configureOptions)
        {
            // Event Store из библиотеки CommandRepository.Mongo
            services.AddEventStoreMongo<IChat, IChatAnemicModel>(configureOptions);

            // Общие Chat-компоненты (CommandDbContext, Projections, IRepository)
            services.AddChatCommandDb(connectionString);

            return services;
        }
    }
}
