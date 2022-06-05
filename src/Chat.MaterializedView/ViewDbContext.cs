using Chat.InternalContracts;
using Chat.MaterializedView.Dialogs;
using Microsoft.EntityFrameworkCore;

namespace Chat.MaterializedView
{
    internal class ViewDbContext: DbContext
    {
        public ViewDbContext(DbContextOptions<DbContext> options): base(options)
            => Database.Migrate();

        public DbSet<DialogView> Dialogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .HasAnnotation("ProductVersion", "0.0.1")
                .HasDefaultSchema("Queries")
                .ApplyConfiguration(new DialogViewMapping());
    }
}
