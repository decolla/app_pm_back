using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app_pm_back.Migrations.PostgresMigration
{
    /// <inheritdoc />
    public partial class AddRuleClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PM_RuleClassification",
                columns: table => new
                {
                    IdRegra = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalavraChave = table.Column<string>(type: "text", nullable: false),
                    CampoAlvo = table.Column<string>(type: "text", nullable: false),
                    ClassificacaoAplicada = table.Column<int>(type: "integer", nullable: false),
                    OrdemPrioridade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_RuleClassification", x => x.IdRegra);
                });

            migrationBuilder.UpdateData(
                table: "PM_ConfiguracoesGlobais",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataModificacao",
                value: new DateTime(2026, 7, 13, 20, 41, 58, 237, DateTimeKind.Utc).AddTicks(9688));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PM_RuleClassification");

            migrationBuilder.UpdateData(
                table: "PM_ConfiguracoesGlobais",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataModificacao",
                value: new DateTime(2026, 7, 9, 20, 27, 44, 157, DateTimeKind.Utc).AddTicks(4749));
        }
    }
}
