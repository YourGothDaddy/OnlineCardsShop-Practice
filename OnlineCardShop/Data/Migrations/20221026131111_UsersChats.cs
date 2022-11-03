using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCardShop.Data.Migrations
{
    public partial class UsersChats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChats_AspNetUsers_UserId",
                table: "UserChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChats_Chats_ChatId",
                table: "UserChats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChats",
                table: "UserChats");

            migrationBuilder.RenameTable(
                name: "UserChats",
                newName: "UsersChats");

            migrationBuilder.RenameIndex(
                name: "IX_UserChats_ChatId",
                table: "UsersChats",
                newName: "IX_UsersChats_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersChats",
                table: "UsersChats",
                columns: new[] { "UserId", "ChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsersChats_AspNetUsers_UserId",
                table: "UsersChats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersChats_Chats_ChatId",
                table: "UsersChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersChats_AspNetUsers_UserId",
                table: "UsersChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersChats_Chats_ChatId",
                table: "UsersChats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersChats",
                table: "UsersChats");

            migrationBuilder.RenameTable(
                name: "UsersChats",
                newName: "UserChats");

            migrationBuilder.RenameIndex(
                name: "IX_UsersChats_ChatId",
                table: "UserChats",
                newName: "IX_UserChats_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChats",
                table: "UserChats",
                columns: new[] { "UserId", "ChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserChats_AspNetUsers_UserId",
                table: "UserChats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChats_Chats_ChatId",
                table: "UserChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
