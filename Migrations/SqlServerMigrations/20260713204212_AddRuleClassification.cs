using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class AddRuleClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PM_FerramentasOperacoes_PM_Operacoes_IdTarefa",
                schema: "dbo",
                table: "PM_FerramentasOperacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConfiguracaoGlobalModel",
                schema: "dbo",
                table: "ConfiguracaoGlobalModel");

            migrationBuilder.RenameTable(
                name: "ConfiguracaoGlobalModel",
                schema: "dbo",
                newName: "PM_ConfiguracoesGlobais",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "IdTarefa",
                schema: "dbo",
                table: "PM_Operacoes",
                newName: "IdOperacao");

            migrationBuilder.RenameColumn(
                name: "IdTarefa",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                newName: "IdOperacao");

            migrationBuilder.RenameIndex(
                name: "IX_PM_FerramentasOperacoes_IdTarefa",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                newName: "IX_PM_FerramentasOperacoes_IdOperacao");

            migrationBuilder.AddColumn<DateTime>(
                name: "FimEsperado",
                schema: "dbo",
                table: "PM_Operacoes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InicioEsperado",
                schema: "dbo",
                table: "PM_Operacoes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PM_ConfiguracoesGlobais",
                schema: "dbo",
                table: "PM_ConfiguracoesGlobais",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PM_RuleClassification",
                schema: "dbo",
                columns: table => new
                {
                    IdRegra = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PalavraChave = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampoAlvo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassificacaoAplicada = table.Column<int>(type: "int", nullable: false),
                    OrdemPrioridade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_RuleClassification", x => x.IdRegra);
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "PM_ConfiguracoesGlobais",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataModificacao",
                value: new DateTime(2026, 7, 13, 20, 42, 12, 4, DateTimeKind.Utc).AddTicks(6790));

            migrationBuilder.AddForeignKey(
                name: "FK_PM_FerramentasOperacoes_PM_Operacoes_IdOperacao",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                column: "IdOperacao",
                principalSchema: "dbo",
                principalTable: "PM_Operacoes",
                principalColumn: "IdOperacao",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PM_FerramentasOperacoes_PM_Operacoes_IdOperacao",
                schema: "dbo",
                table: "PM_FerramentasOperacoes");

            migrationBuilder.DropTable(
                name: "PM_RuleClassification",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PM_ConfiguracoesGlobais",
                schema: "dbo",
                table: "PM_ConfiguracoesGlobais");

            migrationBuilder.DropColumn(
                name: "FimEsperado",
                schema: "dbo",
                table: "PM_Operacoes");

            migrationBuilder.DropColumn(
                name: "InicioEsperado",
                schema: "dbo",
                table: "PM_Operacoes");

            migrationBuilder.RenameTable(
                name: "PM_ConfiguracoesGlobais",
                schema: "dbo",
                newName: "ConfiguracaoGlobalModel",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "IdOperacao",
                schema: "dbo",
                table: "PM_Operacoes",
                newName: "IdTarefa");

            migrationBuilder.RenameColumn(
                name: "IdOperacao",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                newName: "IdTarefa");

            migrationBuilder.RenameIndex(
                name: "IX_PM_FerramentasOperacoes_IdOperacao",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                newName: "IX_PM_FerramentasOperacoes_IdTarefa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConfiguracaoGlobalModel",
                schema: "dbo",
                table: "ConfiguracaoGlobalModel",
                column: "Id");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "ConfiguracaoGlobalModel",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataModificacao",
                value: new DateTime(2026, 7, 9, 18, 19, 1, 21, DateTimeKind.Utc).AddTicks(6270));

            migrationBuilder.AddForeignKey(
                name: "FK_PM_FerramentasOperacoes_PM_Operacoes_IdTarefa",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                column: "IdTarefa",
                principalSchema: "dbo",
                principalTable: "PM_Operacoes",
                principalColumn: "IdTarefa",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
