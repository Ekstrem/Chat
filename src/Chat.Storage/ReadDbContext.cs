using System.Text.Json;
using Chat.InternalContracts;
using Microsoft.EntityFrameworkCore;

namespace Chat.Storage;

/// <summary>
/// DbContext Read Store: проекции <c>messages</c>, <c>inbox</c>, <c>last_read</c>.
/// Схемой управляют EF-миграции этой сборки.
/// </summary>
public sealed class ReadDbContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    /// <summary>MV messages.</summary>
    public DbSet<MessageReadModel> Messages => Set<MessageReadModel>();

    /// <summary>MV inbox.</summary>
    public DbSet<InboxRowReadModel> Inbox => Set<InboxRowReadModel>();

    /// <summary>MV last_read.</summary>
    public DbSet<LastReadReadModel> LastRead => Set<LastReadReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Схема задаётся только для реляционного провайдера: InMemory (тесты) её не поддерживает.
        if (Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
        {
            modelBuilder.HasDefaultSchema("ReadModel");
        }

        modelBuilder.Entity<MessageReadModel>(entity =>
        {
            entity.ToTable("messages");
            entity.HasKey(model => model.Id);
            entity.Property(model => model.Product).HasMaxLength(64).IsRequired();
            entity.Property(model => model.Role).HasMaxLength(64).IsRequired();
            entity.Property(model => model.Text).IsRequired();
            entity.Property(model => model.CreatedAt).HasMaxLength(40);
            entity.Property(model => model.UpdatedAt).HasMaxLength(40);
            entity.HasIndex(model => model.Timestamp);
            entity.Property(model => model.To)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, JsonOptions),
                    value => JsonSerializer.Deserialize<List<string>>(value, JsonOptions) ?? new List<string>());
            entity.Property(model => model.Attachments)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, JsonOptions),
                    value => JsonSerializer.Deserialize<List<Guid>>(value, JsonOptions) ?? new List<Guid>());
            entity.Property(model => model.ExtendedFields)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, JsonOptions),
                    value => JsonSerializer.Deserialize<Dictionary<string, string>>(value, JsonOptions)
                             ?? new Dictionary<string, string>());
            entity.Property(model => model.ReadReceipts)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, JsonOptions),
                    value => JsonSerializer.Deserialize<List<ReadReceiptView>>(value, JsonOptions)
                             ?? new List<ReadReceiptView>());
        });

        modelBuilder.Entity<InboxRowReadModel>(entity =>
        {
            entity.ToTable("inbox");
            entity.HasKey(model => model.Id);
            entity.Property(model => model.Role).HasMaxLength(64).IsRequired();
            entity.Property(model => model.From).HasMaxLength(64).IsRequired();
            entity.Property(model => model.Text).IsRequired();
            entity.HasIndex(model => new { model.Role, model.Timestamp });
            entity.HasIndex(model => model.MessageId);
        });

        modelBuilder.Entity<LastReadReadModel>(entity =>
        {
            entity.ToTable("last_read");
            entity.HasKey(model => model.Id);
            entity.Property(model => model.Role).HasMaxLength(64).IsRequired();
            entity.HasIndex(model => model.Role);
        });
    }
}
