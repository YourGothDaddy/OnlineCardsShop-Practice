using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class DealersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DealerId",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Dealers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dealers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dealers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dealers_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DealerId",
                table: "Cards",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_UserId",
                table: "Dealers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_UserId1",
                table: "Dealers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Dealers_DealerId",
                table: "Cards",
                column: "DealerId",
                principalTable: "Dealers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Dealers_DealerId",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Cards_DealerId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "DealerId",
                table: "Cards");
        }
    }
}
