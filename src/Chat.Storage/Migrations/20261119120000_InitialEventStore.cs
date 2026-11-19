using DigiTFactory.Libraries.CommandRepository.Postgres;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Storage.Migrations;

/// <summary>
/// Схема Command DB: журнал доменных событий, снапшоты и состояния агрегатов.
/// </summary>
[DbContext(typeof(EventStoreDbContext))]
[Migration("20261119120000_InitialEventStore")]
public partial class InitialEventStore : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS \"Commands\";");

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "Commands"."DomainEvents" (
                "Id" uuid NOT NULL,
                "Version" bigint NOT NULL,
                "CorrelationToken" uuid NOT NULL,
                "BoundedContext" character varying(256) NOT NULL,
                "CommandName" character varying(256) NOT NULL,
                "SubjectName" character varying(256) NOT NULL,
                "ChangedValueObjectsJson" jsonb NULL,
                "Result" character varying(1024) NULL,
                "CreatedAt" timestamp with time zone NOT NULL,
                CONSTRAINT "PK_DomainEvents" PRIMARY KEY ("Id", "Version")
            );
            """);

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "Commands"."Snapshots" (
                "Id" uuid NOT NULL,
                "Version" bigint NOT NULL,
                "AggregateJson" jsonb NOT NULL,
                "CreatedAt" timestamp with time zone NOT NULL,
                CONSTRAINT "PK_Snapshots" PRIMARY KEY ("Id", "Version")
            );
            """);

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "Commands"."AggregateStates" (
                "Id" uuid NOT NULL,
                "Version" bigint NOT NULL,
                "AggregateJson" jsonb NOT NULL,
                "UpdatedAt" timestamp with time zone NOT NULL,
                CONSTRAINT "PK_AggregateStates" PRIMARY KEY ("Id")
            );
            """);

        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_DomainEvents_CorrelationToken\" ON \"Commands\".\"DomainEvents\" (\"CorrelationToken\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_DomainEvents_CreatedAt\" ON \"Commands\".\"DomainEvents\" (\"CreatedAt\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_Snapshots_Id\" ON \"Commands\".\"Snapshots\" (\"Id\");");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"Commands\".\"AggregateStates\";");
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"Commands\".\"Snapshots\";");
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"Commands\".\"DomainEvents\";");
    }
}
