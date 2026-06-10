using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class FixedComponentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusOrdem",
                schema: "public",
                table: "PM_Ordens",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusNota",
                schema: "public",
                table: "PM_Notas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusOrdem",
                schema: "public",
                table: "PM_Ordens",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StatusNota",
                schema: "public",
                table: "PM_Notas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
