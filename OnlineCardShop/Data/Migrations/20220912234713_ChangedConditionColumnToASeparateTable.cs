using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class ChangedConditionColumnToASeparateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "Cards",
                newName: "ConditionId");

            migrationBuilder.CreateTable(
                name: "Condition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condition", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ConditionId",
                table: "Cards",
                column: "ConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Condition_ConditionId",
                table: "Cards",
                column: "ConditionId",
                principalTable: "Condition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Condition_ConditionId",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "Condition");

            migrationBuilder.DropIndex(
                name: "IX_Cards_ConditionId",
                table: "Cards");

            migrationBuilder.RenameColumn(
                name: "ConditionId",
                table: "Cards",
                newName: "Condition");
        }
    }
}
