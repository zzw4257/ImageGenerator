using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityFeaturesFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteCount",
                table: "Presets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Presets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PresetFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PresetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresetFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresetFavorites_Presets_PresetId",
                        column: x => x.PresetId,
                        principalTable: "Presets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PresetFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresetLikes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PresetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresetLikes", x => new { x.UserId, x.PresetId });
                    table.ForeignKey(
                        name: "FK_PresetLikes_Presets_PresetId",
                        column: x => x.PresetId,
                        principalTable: "Presets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PresetLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresetReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PresetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReporterUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PresetNameSnapshot = table.Column<string>(type: "TEXT", nullable: false),
                    PresetDescriptionSnapshot = table.Column<string>(type: "TEXT", nullable: true),
                    PresetCoverUrlSnapshot = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresetReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresetReports_Presets_PresetId",
                        column: x => x.PresetId,
                        principalTable: "Presets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PresetReports_Users_ReporterUserId",
                        column: x => x.ReporterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-00000000000a"),
                columns: new[] { "FavoriteCount", "LikeCount" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-00000000000b"),
                columns: new[] { "FavoriteCount", "LikeCount" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-00000000000c"),
                columns: new[] { "FavoriteCount", "LikeCount" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-00000000000d"),
                columns: new[] { "FavoriteCount", "LikeCount" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-00000000000e"),
                columns: new[] { "FavoriteCount", "LikeCount" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-00000000000f"),
                columns: new[] { "FavoriteCount", "LikeCount" },
                values: new object[] { 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_PresetFavorites_PresetId",
                table: "PresetFavorites",
                column: "PresetId");

            migrationBuilder.CreateIndex(
                name: "IX_PresetFavorites_UserId",
                table: "PresetFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PresetLikes_PresetId",
                table: "PresetLikes",
                column: "PresetId");

            migrationBuilder.CreateIndex(
                name: "IX_PresetReports_PresetId",
                table: "PresetReports",
                column: "PresetId");

            migrationBuilder.CreateIndex(
                name: "IX_PresetReports_ReporterUserId",
                table: "PresetReports",
                column: "ReporterUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PresetFavorites");

            migrationBuilder.DropTable(
                name: "PresetLikes");

            migrationBuilder.DropTable(
                name: "PresetReports");

            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "Presets");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Presets");
        }
    }
}
