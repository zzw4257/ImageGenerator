using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class RestoreNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_UserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PresetLikes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PresetLikes",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PresetLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RoleLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperatorUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OldRole = table.Column<int>(type: "INTEGER", nullable: false),
                    NewRole = table.Column<int>(type: "INTEGER", nullable: false),
                    OperationType = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleLogs_Users_OperatorUserId",
                        column: x => x.OperatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleLogs_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleLogs_OperatorUserId",
                table: "RoleLogs",
                column: "OperatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleLogs_TargetUserId",
                table: "RoleLogs",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords",
                column: "PresetId",
                principalTable: "Presets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_UserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets");

            migrationBuilder.DropTable(
                name: "RoleLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PresetLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PresetLikes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PresetLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords",
                column: "PresetId",
                principalTable: "Presets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Presets_Users_CreatedByUserId",
                table: "Presets",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
