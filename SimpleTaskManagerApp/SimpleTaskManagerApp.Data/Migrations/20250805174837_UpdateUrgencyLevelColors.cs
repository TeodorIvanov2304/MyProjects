using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTaskManagerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUrgencyLevelColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UrgencyLevels",
                keyColumn: "Id",
                keyValue: 1,
                column: "Color",
                value: "#28a745");

            migrationBuilder.UpdateData(
                table: "UrgencyLevels",
                keyColumn: "Id",
                keyValue: 2,
                column: "Color",
                value: "#ffc107");

            migrationBuilder.UpdateData(
                table: "UrgencyLevels",
                keyColumn: "Id",
                keyValue: 3,
                column: "Color",
                value: "#dc3545");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UrgencyLevels",
                keyColumn: "Id",
                keyValue: 1,
                column: "Color",
                value: "Green");

            migrationBuilder.UpdateData(
                table: "UrgencyLevels",
                keyColumn: "Id",
                keyValue: 2,
                column: "Color",
                value: "Orange");

            migrationBuilder.UpdateData(
                table: "UrgencyLevels",
                keyColumn: "Id",
                keyValue: 3,
                column: "Color",
                value: "Red");
        }
    }
}
