using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class newBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PM_Ctms_PM_Gpms_IdGPM",
                schema: "public",
                table: "PM_Ctms");

            migrationBuilder.RenameColumn(
                name: "IdGPM",
                schema: "public",
                table: "PM_Ctms",
                newName: "IdCTM");

            migrationBuilder.AddColumn<string>(
                name: "IdLi",
                schema: "public",
                table: "PM_Planos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdAtivo",
                schema: "public",
                table: "PM_Imobilizados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdLi",
                schema: "public",
                table: "PM_Imobilizados",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDados",
                schema: "public",
                table: "DadosRenovacao",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PM_Ctms_PM_Gpms_IdCTM",
                schema: "public",
                table: "PM_Ctms",
                column: "IdCTM",
                principalSchema: "public",
                principalTable: "PM_Gpms",
                principalColumn: "IdGPM",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PM_Ctms_PM_Gpms_IdCTM",
                schema: "public",
                table: "PM_Ctms");

            migrationBuilder.DropColumn(
                name: "IdLi",
                schema: "public",
                table: "PM_Planos");

            migrationBuilder.DropColumn(
                name: "IdAtivo",
                schema: "public",
                table: "PM_Imobilizados");

            migrationBuilder.DropColumn(
                name: "IdLi",
                schema: "public",
                table: "PM_Imobilizados");

            migrationBuilder.DropColumn(
                name: "TipoDados",
                schema: "public",
                table: "DadosRenovacao");

            migrationBuilder.RenameColumn(
                name: "IdCTM",
                schema: "public",
                table: "PM_Ctms",
                newName: "IdGPM");

            migrationBuilder.AddForeignKey(
                name: "FK_PM_Ctms_PM_Gpms_IdGPM",
                schema: "public",
                table: "PM_Ctms",
                column: "IdGPM",
                principalSchema: "public",
                principalTable: "PM_Gpms",
                principalColumn: "IdGPM",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
