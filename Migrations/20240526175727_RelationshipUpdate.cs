using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTUAuthService.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_owner_id",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_owner_id",
                table: "accounts");

            migrationBuilder.AlterColumn<string>(
                name: "owner_id",
                table: "accounts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "accounts",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_UserId",
                table: "accounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_UserId",
                table: "accounts",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_UserId",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_UserId",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "accounts");

            migrationBuilder.AlterColumn<string>(
                name: "owner_id",
                table: "accounts",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_owner_id",
                table: "accounts",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_owner_id",
                table: "accounts",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
