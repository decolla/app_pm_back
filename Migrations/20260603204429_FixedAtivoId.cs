using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class FixedAtivoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ativoAPM",
                schema: "public",
                table: "PM_Ordens");

            migrationBuilder.DropColumn(
                name: "ativoAPM",
                schema: "public",
                table: "PM_Notas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ativoAPM",
                schema: "public",
                table: "PM_Ordens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativoAPM",
                schema: "public",
                table: "PM_Notas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
