using Chat.Storage.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Chat.Storage
{
    public class CommandDbContext : DbContext
    {
        public CommandDbContext(DbContextOptions<CommandDbContext> options) : base(options)
        {
        }

        public DbSet<DomainEventEventEntry> Events { get; set; }

        public DbSet<ChatReadModel> ChatReadModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .HasDefaultSchema("Commands")
                .ApplyConfiguration(new DomainEventMapping())
                .ApplyConfiguration(new ChatReadModelConfiguration());
    }
}
