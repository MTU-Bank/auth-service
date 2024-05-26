using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTUAuthService.Migrations
{
    /// <inheritdoc />
    public partial class FixingAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "accounts",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
               name: "balance",
               table: "accounts",
               type: "bigint(20)");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_UserId",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_UserId",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "balance",
                table: "accounts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "accounts",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_Id",
                table: "accounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_Id",
                table: "accounts",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
