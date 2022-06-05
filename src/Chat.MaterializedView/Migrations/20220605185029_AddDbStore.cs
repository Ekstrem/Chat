using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.MaterializedView.Migrations
{
    public partial class AddDbStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Queries");

            migrationBuilder.CreateTable(
                name: "Dialogs",
                schema: "Queries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dialogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dialogs",
                schema: "Queries");
        }
    }
}
