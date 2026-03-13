using Microsoft.EntityFrameworkCore;

namespace Chat.Storage
{
    public sealed class CommandDbContext : DbContext
    {
        public CommandDbContext(DbContextOptions<CommandDbContext> options) : base(options)
        {
        }

        public DbSet<DomainEventEventEntry> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .HasAnnotation("ProductVersion", "0.0.1")
                .HasDefaultSchema("Commands")
                .ApplyConfiguration(new DomainEventMapping());

    }
}
