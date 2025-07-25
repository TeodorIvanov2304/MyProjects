using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTaskManagerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditIntUrgencyLevelId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_UrgencyLevels_UrgencyLevelId",
                table: "AppTasks");

            migrationBuilder.AlterColumn<int>(
                name: "UrgencyLevelId",
                table: "AppTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Urgency level identifier",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "Urgency level identifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_UrgencyLevels_UrgencyLevelId",
                table: "AppTasks",
                column: "UrgencyLevelId",
                principalTable: "UrgencyLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_UrgencyLevels_UrgencyLevelId",
                table: "AppTasks");

            migrationBuilder.AlterColumn<int>(
                name: "UrgencyLevelId",
                table: "AppTasks",
                type: "integer",
                nullable: true,
                comment: "Urgency level identifier",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Urgency level identifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_UrgencyLevels_UrgencyLevelId",
                table: "AppTasks",
                column: "UrgencyLevelId",
                principalTable: "UrgencyLevels",
                principalColumn: "Id");
        }
    }
}
