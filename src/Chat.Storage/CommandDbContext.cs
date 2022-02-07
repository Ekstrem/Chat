using Microsoft.EntityFrameworkCore;

namespace Chat.Storage
{
    internal sealed class CommandDbContext : DbContext
    {
        public CommandDbContext(DbContextOptions<DbContext> options) : base(options)
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
