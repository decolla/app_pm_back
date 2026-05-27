using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class DtoFix3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "NumeroOrdem",
                schema: "public",
                table: "PM_Ordens",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ConfiabilidadeRestaurada",
                schema: "public",
                table: "PM_Ordens",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NumeroOrdem",
                schema: "public",
                table: "PM_Ordens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ConfiabilidadeRestaurada",
                schema: "public",
                table: "PM_Ordens",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
