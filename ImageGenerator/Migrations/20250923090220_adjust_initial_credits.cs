using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class adjust_initial_credits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "Credits", "LastCreditClaimedAt" },
                values: new object[] { 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "Credits", "LastCreditClaimedAt" },
                values: new object[] { 0, null });
        }
    }
}
