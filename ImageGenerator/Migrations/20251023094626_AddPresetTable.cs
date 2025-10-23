using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddPresetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PresetId",
                table: "GenerationRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Presets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CoverUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: false),
                    PriceCredits = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultParams = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenerationRecords_PresetId",
                table: "GenerationRecords",
                column: "PresetId");

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords",
                column: "PresetId",
                principalTable: "Presets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords");

            migrationBuilder.DropTable(
                name: "Presets");

            migrationBuilder.DropIndex(
                name: "IX_GenerationRecords_PresetId",
                table: "GenerationRecords");

            migrationBuilder.DropColumn(
                name: "PresetId",
                table: "GenerationRecords");
        }
    }
}
