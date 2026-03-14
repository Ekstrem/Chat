using Chat.DomainServices;
using Chat.Storage.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Storage
{
    /// <summary>
    /// Регистрация общих Chat-компонентов (CommandDbContext, Projections, IRepository).
    /// Вызывается из Chat.Storage.Postgres или Chat.Storage.Mongo.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует CommandDbContext, ChatProjectionService и IRepository.
        /// Предполагается, что IAnemicModelRepository уже зарегистрирован
        /// конкретной СУБД-библиотекой (Postgres или Mongo).
        /// </summary>
        public static IServiceCollection AddChatCommandDb(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<CommandDbContext>(options =>
                options.UseNpgsql(connectionString, b =>
                    b.MigrationsAssembly("Chat.Storage")));

            services.AddScoped<ChatProjectionService>();
            services.AddScoped<IRepository, Repository>();

            return services;
        }
    }
}
