using Chat.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Storage.Migrations;

/// <summary>
/// Схема Read Store: проекции messages, inbox, last_read.
/// </summary>
[DbContext(typeof(ReadDbContext))]
[Migration("20261119121000_InitialReadStore")]
public partial class InitialReadStore : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS \"ReadModel\";");

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "ReadModel"."messages" (
                "Id" uuid NOT NULL,
                "Product" character varying(64) NOT NULL,
                "Role" character varying(64) NOT NULL,
                "Timestamp" bigint NOT NULL,
                "To" text NOT NULL,
                "Text" text NOT NULL,
                "Attachments" text NOT NULL,
                "ReplyTo" uuid NULL,
                "ExtendedFields" text NOT NULL,
                "ReadReceipts" text NOT NULL,
                "Version" bigint NOT NULL,
                "CreatedAt" character varying(40) NOT NULL,
                "UpdatedAt" character varying(40) NOT NULL,
                CONSTRAINT "PK_messages" PRIMARY KEY ("Id")
            );
            """);

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "ReadModel"."inbox" (
                "Id" uuid NOT NULL,
                "Role" character varying(64) NOT NULL,
                "MessageId" uuid NOT NULL,
                "From" character varying(64) NOT NULL,
                "Text" text NOT NULL,
                "Timestamp" bigint NOT NULL,
                "ReadAt" bigint NULL,
                CONSTRAINT "PK_inbox" PRIMARY KEY ("Id")
            );
            """);

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "ReadModel"."last_read" (
                "Id" uuid NOT NULL,
                "Role" character varying(64) NOT NULL,
                "LastReadAt" bigint NULL,
                CONSTRAINT "PK_last_read" PRIMARY KEY ("Id")
            );
            """);

        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_messages_Timestamp\" ON \"ReadModel\".\"messages\" (\"Timestamp\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_inbox_Role_Timestamp\" ON \"ReadModel\".\"inbox\" (\"Role\", \"Timestamp\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_inbox_MessageId\" ON \"ReadModel\".\"inbox\" (\"MessageId\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_last_read_Role\" ON \"ReadModel\".\"last_read\" (\"Role\");");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"ReadModel\".\"last_read\";");
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"ReadModel\".\"inbox\";");
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"ReadModel\".\"messages\";");
    }
}
