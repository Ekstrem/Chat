using Chat.Storage.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Chat.Storage
{
    /// <summary>
    /// Chat-специфичный DbContext для Read Models.
    /// Таблица Events теперь управляется EventStoreDbContext из библиотеки CommandRepository.Postgres.
    /// </summary>
    public class CommandDbContext : DbContext
    {
        public CommandDbContext(DbContextOptions<CommandDbContext> options) : base(options)
        {
        }

        public DbSet<ChatReadModel> ChatReadModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .HasDefaultSchema("Commands")
                .ApplyConfiguration(new ChatReadModelConfiguration());
    }
}
