using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddChatReadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatReadModels",
                schema: "Commands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SubscriberName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OperatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MessageCount = table.Column<int>(type: "integer", nullable: false),
                    LastCommandName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatReadModels_Status",
                schema: "Commands",
                table: "ChatReadModels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ChatReadModels_UpdatedAt",
                schema: "Commands",
                table: "ChatReadModels",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatReadModels",
                schema: "Commands");
        }
    }
}
