using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class FixingRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Cards_CardId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_CardId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                table: "Images",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ImageId",
                table: "Cards",
                column: "ImageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Images_ImageId",
                table: "Cards",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Images_ImageId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_ImageId",
                table: "Cards");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_CardId",
                table: "Images",
                column: "CardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Cards_CardId",
                table: "Images",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
