using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTaskManagerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAppTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Unique task identifier"),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Task title"),
                    Description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false, comment: "Task description"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Start date of the task"),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Task due date"),
                    UserId = table.Column<string>(type: "text", nullable: false, comment: "User identifier")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTasks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTasks_UserId",
                table: "AppTasks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTasks");
        }
    }
}
