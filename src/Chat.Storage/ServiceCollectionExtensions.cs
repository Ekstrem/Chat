using Chat.DomainServices;
using Chat.Storage.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatStorage(this IServiceCollection services, string connectionString)
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
