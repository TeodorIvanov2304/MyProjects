using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTaskManagerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppTasks",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Whether the task is soft-deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppTasks");
        }
    }
}
