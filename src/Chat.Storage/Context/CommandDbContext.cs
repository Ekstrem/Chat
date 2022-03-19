using Chat.Domain;
using Chat.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Chat.Storage
{
    internal sealed class CommandDbContext : DbContext
    {
        public CommandDbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        //public DbSet<SlobEntry<IChatAnemicModel, IChat>> Entries { get; set; }
        public DbSet<DomainEventEntry> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .HasAnnotation("ProductVersion", "0.0.1")
                .HasDefaultSchema("Commands")
                //.ApplyConfiguration(new SlobMapping())
                .ApplyConfiguration(new DomainEventMapping());

    }
}
