using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTUAuthService.Migrations
{
    /// <inheritdoc />
    public partial class TokenVarCharIncrease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "tokens",
                type: "varchar(768)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2048)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "tokens",
                type: "varchar(2048)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(768)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
