using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Storage.Migrations
{
    public partial class AddDbStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Commands");

            migrationBuilder.CreateTable(
                name: "DomainEvents",
                schema: "Commands",
                columns: table => new
                {
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    CommandVersion = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommandName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubjectName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValueObjects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvents", x => new { x.AggregateId, x.AggregateVersion, x.CommandVersion });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents",
                schema: "Commands");
        }
    }
}
