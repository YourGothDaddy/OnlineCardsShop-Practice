using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class AddedPriceToCards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Cards",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Cards");
        }
    }
}
