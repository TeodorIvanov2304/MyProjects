using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTaskManagerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "LogEntries");

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "LogEntries",
                type: "text",
                nullable: true,
                comment: "The name or title of the affected entity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "LogEntries");

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "LogEntries",
                type: "text",
                nullable: true,
                comment: "The ID of the object");
        }
    }
}
