using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class DnsIot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DnsIot",
                schema: "public",
                table: "PM_Ordens",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnsIot",
                schema: "public",
                table: "PM_Notas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DnsIot",
                schema: "public",
                table: "PM_Ordens");

            migrationBuilder.DropColumn(
                name: "DnsIot",
                schema: "public",
                table: "PM_Notas");
        }
    }
}
