using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTaskManagerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class LogEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Unique log identifier"),
                    UserId = table.Column<string>(type: "text", nullable: false, comment: "User identifier"),
                    UserEmail = table.Column<string>(type: "text", nullable: false, comment: "User email address"),
                    Action = table.Column<string>(type: "text", nullable: false, comment: "Action performed by the user"),
                    EntityType = table.Column<string>(type: "text", nullable: false, comment: "The type of object that was changed or used."),
                    EntityId = table.Column<string>(type: "text", nullable: true, comment: "The ID of the object"),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Timestamp of the current log")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries");
        }
    }
}
