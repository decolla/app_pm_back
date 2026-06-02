using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdLi",
                schema: "public",
                table: "DadosRenovacao",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdLi",
                schema: "public",
                table: "DadosRenovacao");
        }
    }
}
