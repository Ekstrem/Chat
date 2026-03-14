using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Storage.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Commands");

            migrationBuilder.CreateTable(
                name: "DomainEvents",
                schema: "Commands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CorrelationToken = table.Column<Guid>(type: "uuid", nullable: false),
                    BoundedContext = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CommandName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ChangedValueObjectsJson = table.Column<string>(type: "jsonb", nullable: true),
                    Result = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvents", x => new { x.Id, x.Version });
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_CorrelationToken",
                schema: "Commands",
                table: "DomainEvents",
                column: "CorrelationToken");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_CreatedAt",
                schema: "Commands",
                table: "DomainEvents",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents",
                schema: "Commands");
        }
    }
}
