using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddPresets_SeedData_AndLinkToGenerationRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords");

            migrationBuilder.InsertData(
                table: "Presets",
                columns: new[] { "Id", "CoverUrl", "CreatedAt", "DefaultParams", "IsDeleted", "Name", "PriceCredits", "Prompt", "Provider", "Tags" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "/images/presets/product-shot.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"cinematic\", \"width\": 1024, \"height\": 1024}", false, "产品商业摄影 (Qwen)", 2, "A high-resolution, studio-lit product photograph of a [product description] on a [background surface]. The lighting is a [lighting setup] to emphasize subtle curves. Ultra-realistic, 4k.", "Qwen", "[\"product\",\"cinematic\",\"qwen\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "/images/presets/neon-shoe.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"aspectRatio\": \"3:2\", \"width\": 768, \"height\": 512}", false, "霓虹风格 (Flux)", 1, "a neon-lit product photograph of a sneaker on glossy floor, cinematic lighting, high contrast, 4k, [subject]", "Flux", "[\"product\",\"neon\",\"flux\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "/images/presets/sticker.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"sticker\", \"width\": 512, \"height\": 512}", false, "风格化贴纸 (Stub)", 0, "A kawaii chibi sticker of a [subject], clean bold outline, soft cell shading, transparent background.", "Stub", "[\"sticker\",\"chibi\",\"stub\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "/images/presets/photorealistic.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"aspectRatio\": \"16:9\", \"width\": 1024, \"height\": 576}", false, "逼真摄影 (Qwen)", 2, "A photorealistic close-up of [subject], set in [environment]. The scene is illuminated by [lighting description], creating a serene atmosphere. Captured with a Canon EOS R5.", "Qwen", "[\"photo\",\"realistic\",\"qwen\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "/images/presets/minimal.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"aspectRatio\": \"3:2\", \"width\": 768, \"height\": 512}", false, "极简负空间 (Flux)", 1, "A minimalist composition featuring a single [subject] positioned in the lower right. The background is a vast, empty off-white canvas, creating significant negative space.", "Flux", "[\"minimalist\",\"art\",\"flux\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "/images/presets/comic.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"comic\", \"width\": 512, \"height\": 768}", false, "漫画单格 (Stub)", 0, "A single comic book panel in a neo-noir ink wash style. In the foreground, [character description]. In the background, [setting details].", "Stub", "[\"comic\",\"noir\",\"stub\"]" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords",
                column: "PresetId",
                principalTable: "Presets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords");

            migrationBuilder.DeleteData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000c"));

            migrationBuilder.DeleteData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000d"));

            migrationBuilder.DeleteData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000e"));

            migrationBuilder.DeleteData(
                table: "Presets",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000f"));

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_Presets_PresetId",
                table: "GenerationRecords",
                column: "PresetId",
                principalTable: "Presets",
                principalColumn: "Id");
        }
    }
}
