using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddPresetFeature : Migration
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
                    Description = table.Column<string>(type: "TEXT", nullable: false),
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

            migrationBuilder.InsertData(
                table: "Presets",
                columns: new[] { "Id", "CoverUrl", "CreatedAt", "DefaultParams", "Description", "IsDeleted", "Name", "PriceCredits", "Prompt", "Provider", "Tags" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "/images/presets/product-shot.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"photorealistic\", \"width\": 1024, \"height\": 1024, \"aspectRatio\": \"1:1\"}", "适合电商/广告用途的专业产品照片，强调光线布置、角度与核心细节。", false, "产品商业摄影 (Qwen)", 2, "A high-resolution, studio-lit product photograph of a [product description:matte black wireless earbud case] on a [background surface/description:brushed aluminum surface with soft vignette]. The lighting is a [lighting setup:three-point softbox] to [lighting purpose:emphasize subtle curves]. The camera angle is a [angle type:slight low angle] to showcase [specific feature:charging indicator + hinge]. Ultra-realistic, with sharp focus on [key detail:texture + logo etching]. [Aspect ratio:1:1].", "Qwen", "[\"product\",\"studio\",\"qwen\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "/images/presets/text-graphic.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"graphic\", \"width\": 768, \"height\": 768, \"aspectRatio\": \"1:1\"}", "用于生成包含特定文字的图形 / 标识，明确字体感受、风格与配色。", false, "文字图形标识 (Flux)", 1, "Create a [image type:logo badge] for [brand/concept:Arctic Labs] with the text \"[text to render:POLAR AI]\" in a [font style:geometric sans-serif]. The design should be [style description:minimal, futuristic] with a [color scheme:icy blue + white gradient].", "Flux", "[\"text\",\"logo\",\"graphic\",\"flux\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "/images/presets/sticker.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"sticker\", \"width\": 512, \"height\": 512, \"aspectRatio\": \"1:1\"}", "用于创建带有指定风格的贴纸 / 图标素材，强调线条、配色与透明背景。", false, "风格化贴纸 (Stub)", 0, "A [style:kawaii chibi] sticker of a [subject:cat astronaut], featuring [key characteristics:round helmet, floating fish] and a [color palette:pastel neon mix]. The design should have [line style:clean bold outline] and [shading style:soft cell shading]. The background must be transparent.", "Stub", "[\"sticker\",\"chibi\",\"icon\",\"stub\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "/images/presets/photorealistic.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"photorealistic\", \"width\": 1024, \"height\": 576, \"aspectRatio\": \"16:9\"}", "对于逼真的图片，请使用摄影术语。提及拍摄角度、镜头类型、光线和细节，引导模型生成逼真的效果。", false, "逼真摄影场景 (Qwen)", 2, "A photorealistic [shot type:close-up] of [subject:a mystical fox], [action or expression:looking into the distance], set in [environment:ancient forest]. The scene is illuminated by [lighting description:soft golden hour rim light], creating a [mood:serene] atmosphere. Captured with a [camera/lens details:Canon EOS R5 + 85mm f1.2], emphasizing [key textures and details:detailed fur, shimmering particles]. The image should be in a [aspect ratio:16:9] format.", "Qwen", "[\"photo\",\"realistic\",\"camera\",\"qwen\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "/images/presets/minimal.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"minimalist\", \"width\": 768, \"height\": 512, \"aspectRatio\": \"3:2\"}", "生成带大量留白与单主体的极简风图像，适合做背景或叠加文案。", false, "极简负空间 (Flux)", 1, "A minimalist composition featuring a single [subject:solitary bonsai] positioned in the [position in frame:lower right] of the frame. The background is a vast, empty [color:off-white] canvas, creating significant negative space. Soft, subtle lighting. [Aspect ratio:3:2].", "Flux", "[\"minimalist\",\"art\",\"negative space\",\"flux\"]" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "/images/presets/comic.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"style\": \"comic\", \"width\": 512, \"height\": 910, \"aspectRatio\": \"9:16\"}", "生成漫画风单格场景，分离前景角色动作与背景设定，可含对白框。", false, "漫画单格 (Stub)", 0, "A single comic book panel in a [art style:neo-noir ink wash] style. In the foreground, [character description and action:detective leaning over a glowing map]. In the background, [setting details:rain streaked window + neon signs]. The panel has a [dialogue/caption box:caption] with the text \"[Text:We were already too late]\". The lighting creates a [mood:brooding] mood. [Aspect ratio:9:16].", "Stub", "[\"comic\",\"noir\",\"storyboard\",\"stub\"]" }
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
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
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
