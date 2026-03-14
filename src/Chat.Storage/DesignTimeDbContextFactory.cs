using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Chat.Storage
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CommandDbContext>
    {
        public CommandDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommandDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=chat_db;Username=chat_user;Password=chat_password",
                b => b.MigrationsAssembly("Chat.Storage"));

            return new CommandDbContext(optionsBuilder.Options);
        }
    }
}
