using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class PathColumnToImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Images");
        }
    }
}
