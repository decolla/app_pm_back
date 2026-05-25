using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class NewOrderUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fluido",
                schema: "public",
                table: "PM_NotasManutencao");

            migrationBuilder.DropColumn(
                name: "MatrizLibra",
                schema: "public",
                table: "PM_NotasManutencao");

            migrationBuilder.DropColumn(
                name: "Plaqueta",
                schema: "public",
                table: "PM_NotasManutencao");

            migrationBuilder.DropColumn(
                name: "Pressao",
                schema: "public",
                table: "PM_NotasManutencao");

            migrationBuilder.DropColumn(
                name: "Temperatura",
                schema: "public",
                table: "PM_NotasManutencao");

            migrationBuilder.DropColumn(
                name: "Tensao",
                schema: "public",
                table: "PM_NotasManutencao");

            migrationBuilder.AlterColumn<string>(
                name: "StatusOrdem",
                schema: "public",
                table: "PM_OrdensManutencao",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusOrdem",
                schema: "public",
                table: "PM_OrdensManutencao",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fluido",
                schema: "public",
                table: "PM_NotasManutencao",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrizLibra",
                schema: "public",
                table: "PM_NotasManutencao",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plaqueta",
                schema: "public",
                table: "PM_NotasManutencao",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pressao",
                schema: "public",
                table: "PM_NotasManutencao",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperatura",
                schema: "public",
                table: "PM_NotasManutencao",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tensao",
                schema: "public",
                table: "PM_NotasManutencao",
                type: "numeric",
                nullable: true);
        }
    }
}
