using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets");

            migrationBuilder.AddForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets");

            migrationBuilder.AddForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
