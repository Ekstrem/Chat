using Microsoft.EntityFrameworkCore;

namespace Chat.Storage.ReadModel
{
    /// <summary>
    /// DbContext для Read-моделей (проекций) чата.
    /// Отдельный контекст от CommandDbContext для разделения Write/Read хранилищ.
    /// </summary>
    public class ReadDbContext : DbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }

        public DbSet<ChatReadModel> ChatProjections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ReadModel");

            modelBuilder.Entity<ChatReadModel>(entity =>
            {
                entity.ToTable("ChatProjections");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.SessionId).IsRequired();
                entity.Property(e => e.ActorLogin).HasMaxLength(256);
                entity.Property(e => e.ActorType).HasMaxLength(64);
                entity.Property(e => e.OperatorLogin).HasMaxLength(256);
                entity.Property(e => e.LastMessageText).HasMaxLength(4000);
                entity.Property(e => e.Status).HasMaxLength(64).IsRequired();
                entity.Property(e => e.FeedbackText).HasMaxLength(4000);
                entity.Property(e => e.LastCommand).HasMaxLength(256);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}
