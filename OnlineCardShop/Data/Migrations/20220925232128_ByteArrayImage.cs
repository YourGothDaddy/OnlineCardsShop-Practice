using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class ByteArrayImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Cards",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Cards");
        }
    }
}
