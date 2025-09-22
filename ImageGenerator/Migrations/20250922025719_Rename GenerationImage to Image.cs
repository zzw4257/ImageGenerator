using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGenerator.Migrations
{
    /// <inheritdoc />
    public partial class RenameGenerationImagetoImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecordImage_GeneratedImages_InputImagesId",
                table: "GenerationRecordImage");

            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_GeneratedImages_OutputImagesId",
                table: "GenerationRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneratedImages",
                table: "GeneratedImages");

            migrationBuilder.RenameTable(
                name: "GeneratedImages",
                newName: "Images");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecordImage_Images_InputImagesId",
                table: "GenerationRecordImage",
                column: "InputImagesId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_Images_OutputImagesId",
                table: "GenerationRecords",
                column: "OutputImagesId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecordImage_Images_InputImagesId",
                table: "GenerationRecordImage");

            migrationBuilder.DropForeignKey(
                name: "FK_GenerationRecords_Images_OutputImagesId",
                table: "GenerationRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "GeneratedImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneratedImages",
                table: "GeneratedImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecordImage_GeneratedImages_InputImagesId",
                table: "GenerationRecordImage",
                column: "InputImagesId",
                principalTable: "GeneratedImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationRecords_GeneratedImages_OutputImagesId",
                table: "GenerationRecords",
                column: "OutputImagesId",
                principalTable: "GeneratedImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
