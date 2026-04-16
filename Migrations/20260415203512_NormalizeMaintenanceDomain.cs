using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeMaintenanceDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemNotaCongas_NotasCongas_NoteCongasIdNota",
                table: "ItemNotaCongas");

            migrationBuilder.DropForeignKey(
                name: "FK_NotasCongas_Equipamentos_EquipmentModelIdEquipamento",
                table: "NotasCongas");

            migrationBuilder.DropIndex(
                name: "IX_NotasCongas_EquipmentModelIdEquipamento",
                table: "NotasCongas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanosManutnecao",
                table: "PlanosManutnecao");

            migrationBuilder.DropColumn(
                name: "EquipmentModelIdEquipamento",
                table: "NotasCongas");

            migrationBuilder.DropColumn(
                name: "ItemManut",
                table: "PlanosManutnecao");

            migrationBuilder.RenameTable(
                name: "PlanosManutnecao",
                newName: "PlanosManutencao");

            migrationBuilder.AlterColumn<string>(
                name: "TipoNota",
                table: "NotasCongas",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusUsuario",
                table: "NotasCongas",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusSistema",
                table: "NotasCongas",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unidade",
                table: "PlanosManutencao",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TxtCicloManut",
                table: "PlanosManutencao",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPM",
                table: "PlanosManutencao",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdEstrategia",
                table: "PlanosManutencao",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanosManutencao",
                table: "PlanosManutencao",
                column: "IdPlano");

            migrationBuilder.CreateTable(
                name: "CentrosTrabalho",
                columns: table => new
                {
                    IdCentroTrabalho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CentroPlanejamento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosTrabalho", x => x.IdCentroTrabalho);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    IdEndereco = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rua = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Estado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Pais = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.IdEndereco);
                });

            migrationBuilder.CreateTable(
                name: "EstrategiaComgasModel",
                columns: table => new
                {
                    IdEstrategia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeEstrategia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UnidadeEstrategica = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HorizonteAbertura = table.Column<int>(type: "integer", nullable: true),
                    FatorDeslocamentoAtraso = table.Column<decimal>(type: "numeric", nullable: true),
                    ToleranciaAtraso = table.Column<decimal>(type: "numeric", nullable: true),
                    FatorDeslocamentoAntecipado = table.Column<decimal>(type: "numeric", nullable: true),
                    ToleranciaAntecipada = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstrategiaComgasModel", x => x.IdEstrategia);
                });

            migrationBuilder.CreateTable(
                name: "EstrategiasManutencao",
                columns: table => new
                {
                    IdEstrategia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CodigoProgramacao = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    UnidadeEstrategica = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HorizonteAbertura = table.Column<int>(type: "integer", nullable: true),
                    FatorDeslocamentoAtrasada = table.Column<decimal>(type: "numeric", nullable: true),
                    ToleranciaAtrasadaPercentual = table.Column<decimal>(type: "numeric", nullable: true),
                    FatorDeslocamentoAntecipada = table.Column<decimal>(type: "numeric", nullable: true),
                    ToleranciaAntecipadaPercentual = table.Column<decimal>(type: "numeric", nullable: true),
                    CalendarioFabrica = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstrategiasManutencao", x => x.IdEstrategia);
                });

            migrationBuilder.CreateTable(
                name: "Fabricantes",
                columns: table => new
                {
                    IdFabricante = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabricantes", x => x.IdFabricante);
                });

            migrationBuilder.CreateTable(
                name: "GruposPlanejamentoManutencao",
                columns: table => new
                {
                    IdGpm = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposPlanejamentoManutencao", x => x.IdGpm);
                });

            migrationBuilder.CreateTable(
                name: "Imobilizados",
                columns: table => new
                {
                    TagImobilizado = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NumeroSerie = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    NumeroInventario = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataIncorporacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValorCompra = table.Column<decimal>(type: "numeric", nullable: true),
                    DepreciacaoAno = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorAtual = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imobilizados", x => x.TagImobilizado);
                });

            migrationBuilder.CreateTable(
                name: "ItemManutencaoComgasModel",
                columns: table => new
                {
                    IdItem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemManut = table.Column<long>(type: "bigint", nullable: true),
                    LocInstalacao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Equipamento = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    PlanoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemManutencaoComgasModel", x => x.IdItem);
                    table.ForeignKey(
                        name: "FK_ItemManutencaoComgasModel_PlanosManutencao_PlanoId",
                        column: x => x.PlanoId,
                        principalTable: "PlanosManutencao",
                        principalColumn: "IdPlano",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosSap",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Senha = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Mandante = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Idioma = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosSap", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Localizacoes",
                columns: table => new
                {
                    IdLocalizacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CenLocalizacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CodigoLocalizacao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EmGarantia = table.Column<bool>(type: "boolean", nullable: true),
                    AreaOperacional = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CodigoAbc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CampoOrdenacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EnderecoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacoes", x => x.IdLocalizacao);
                    table.ForeignKey(
                        name: "FK_Localizacoes_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "IdEndereco");
                });

            migrationBuilder.CreateTable(
                name: "CicloComgasModel",
                columns: table => new
                {
                    IdCiclo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ValorCiclo = table.Column<int>(type: "integer", nullable: true),
                    Unidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NivelManutencao = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Offset = table.Column<int>(type: "integer", nullable: true),
                    IdEstrategia = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CicloComgasModel", x => x.IdCiclo);
                    table.ForeignKey(
                        name: "FK_CicloComgasModel_EstrategiaComgasModel_IdEstrategia",
                        column: x => x.IdEstrategia,
                        principalTable: "EstrategiaComgasModel",
                        principalColumn: "IdEstrategia",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlanosManutencaoNormalizados",
                columns: table => new
                {
                    CodigoPlano = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    NumeroSolicitacao = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UltimaOrdem = table.Column<long>(type: "bigint", nullable: true),
                    EstrategiaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanosManutencaoNormalizados", x => x.CodigoPlano);
                    table.ForeignKey(
                        name: "FK_PlanosManutencaoNormalizados_EstrategiasManutencao_Estrateg~",
                        column: x => x.EstrategiaId,
                        principalTable: "EstrategiasManutencao",
                        principalColumn: "IdEstrategia",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LocaisInstalacao",
                columns: table => new
                {
                    CodigoLocalInstalacao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Denominacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SistemaIdentificado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CodigoEstrutura = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Classe = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TipoObjeto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GrupoAutorizacoes = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Peso = table.Column<decimal>(type: "numeric", nullable: true),
                    NumeroInventario = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TipoNotaPadrao = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ValorAquisicao = table.Column<decimal>(type: "numeric", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FabricanteId = table.Column<int>(type: "integer", nullable: true),
                    LocalizacaoId = table.Column<int>(type: "integer", nullable: true),
                    EnderecoId = table.Column<int>(type: "integer", nullable: true),
                    ImobilizadoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    GpmId = table.Column<int>(type: "integer", nullable: true),
                    CentroTrabalhoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocaisInstalacao", x => x.CodigoLocalInstalacao);
                    table.ForeignKey(
                        name: "FK_LocaisInstalacao_CentrosTrabalho_CentroTrabalhoId",
                        column: x => x.CentroTrabalhoId,
                        principalTable: "CentrosTrabalho",
                        principalColumn: "IdCentroTrabalho",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LocaisInstalacao_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "IdEndereco",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LocaisInstalacao_Fabricantes_FabricanteId",
                        column: x => x.FabricanteId,
                        principalTable: "Fabricantes",
                        principalColumn: "IdFabricante",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LocaisInstalacao_GruposPlanejamentoManutencao_GpmId",
                        column: x => x.GpmId,
                        principalTable: "GruposPlanejamentoManutencao",
                        principalColumn: "IdGpm",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LocaisInstalacao_Imobilizados_ImobilizadoTag",
                        column: x => x.ImobilizadoTag,
                        principalTable: "Imobilizados",
                        principalColumn: "TagImobilizado",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LocaisInstalacao_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalTable: "Localizacoes",
                        principalColumn: "IdLocalizacao",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CiclosManutencao",
                columns: table => new
                {
                    IdCiclo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Valor = table.Column<int>(type: "integer", nullable: false),
                    Unidade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NivelManutencao = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Offset = table.Column<int>(type: "integer", nullable: true),
                    PlanoManutencaoCodigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CiclosManutencao", x => x.IdCiclo);
                    table.ForeignKey(
                        name: "FK_CiclosManutencao_PlanosManutencaoNormalizados_PlanoManutenc~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalTable: "PlanosManutencaoNormalizados",
                        principalColumn: "CodigoPlano",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipamentosAtivos",
                columns: table => new
                {
                    TagEquipamento = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Denominacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ValidoDesde = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidoAte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Classe = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TipoObjeto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GrupoAutorizacoes = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Peso = table.Column<decimal>(type: "numeric", nullable: true),
                    NumeroInventario = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TipoNotaPadrao = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ValorAquisicao = table.Column<decimal>(type: "numeric", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FabricanteId = table.Column<int>(type: "integer", nullable: true),
                    LocalizacaoId = table.Column<int>(type: "integer", nullable: true),
                    EnderecoId = table.Column<int>(type: "integer", nullable: true),
                    ImobilizadoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    GpmId = table.Column<int>(type: "integer", nullable: true),
                    CentroTrabalhoId = table.Column<int>(type: "integer", nullable: true),
                    LocalInstalacaoCodigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipamentosAtivos", x => x.TagEquipamento);
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_CentrosTrabalho_CentroTrabalhoId",
                        column: x => x.CentroTrabalhoId,
                        principalTable: "CentrosTrabalho",
                        principalColumn: "IdCentroTrabalho",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "IdEndereco");
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_Fabricantes_FabricanteId",
                        column: x => x.FabricanteId,
                        principalTable: "Fabricantes",
                        principalColumn: "IdFabricante",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_GruposPlanejamentoManutencao_GpmId",
                        column: x => x.GpmId,
                        principalTable: "GruposPlanejamentoManutencao",
                        principalColumn: "IdGpm",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_Imobilizados_ImobilizadoTag",
                        column: x => x.ImobilizadoTag,
                        principalTable: "Imobilizados",
                        principalColumn: "TagImobilizado",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_LocaisInstalacao_LocalInstalacaoCodigo",
                        column: x => x.LocalInstalacaoCodigo,
                        principalTable: "LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquipamentosAtivos_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalTable: "Localizacoes",
                        principalColumn: "IdLocalizacao",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ItensPlanoManutencao",
                columns: table => new
                {
                    IdItemPlano = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoItem = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PlanoManutencaoCodigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    LocalInstalacaoCodigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPlanoManutencao", x => x.IdItemPlano);
                    table.ForeignKey(
                        name: "FK_ItensPlanoManutencao_EquipamentosAtivos_EquipamentoTag",
                        column: x => x.EquipamentoTag,
                        principalTable: "EquipamentosAtivos",
                        principalColumn: "TagEquipamento",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ItensPlanoManutencao_LocaisInstalacao_LocalInstalacaoCodigo",
                        column: x => x.LocalInstalacaoCodigo,
                        principalTable: "LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ItensPlanoManutencao_PlanosManutencaoNormalizados_PlanoManu~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalTable: "PlanosManutencaoNormalizados",
                        principalColumn: "CodigoPlano",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotasManutencao",
                columns: table => new
                {
                    IdNota = table.Column<long>(type: "bigint", nullable: false),
                    TipoNota = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DescricaoDetalhada = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    StatusNota = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DataNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Prioridade = table.Column<int>(type: "integer", nullable: false),
                    InicioDesejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ConclusaoDesejada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    LocalInstalacaoCodigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    CriadoPorUsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasManutencao", x => x.IdNota);
                    table.ForeignKey(
                        name: "FK_NotasManutencao_EquipamentosAtivos_EquipamentoTag",
                        column: x => x.EquipamentoTag,
                        principalTable: "EquipamentosAtivos",
                        principalColumn: "TagEquipamento",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotasManutencao_LocaisInstalacao_LocalInstalacaoCodigo",
                        column: x => x.LocalInstalacaoCodigo,
                        principalTable: "LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotasManutencao_UsuariosSap_CriadoPorUsuarioId",
                        column: x => x.CriadoPorUsuarioId,
                        principalTable: "UsuariosSap",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AtividadesNota",
                columns: table => new
                {
                    IdAtividade = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadesNota", x => x.IdAtividade);
                    table.ForeignKey(
                        name: "FK_AtividadesNota_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalTable: "NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensNotaManutencao",
                columns: table => new
                {
                    IdItemNota = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    ParteObjeto = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Sintoma = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoSintoma = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Causa = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoCausa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensNotaManutencao", x => x.IdItemNota);
                    table.ForeignKey(
                        name: "FK_ItensNotaManutencao_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalTable: "NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedidasNota",
                columns: table => new
                {
                    IdMedida = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedidasNota", x => x.IdMedida);
                    table.ForeignKey(
                        name: "FK_MedidasNota_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalTable: "NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdensManutencao",
                columns: table => new
                {
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false),
                    TipoOrdem = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StatusOrdem = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NotaRelacionadaId = table.Column<long>(type: "bigint", nullable: true),
                    PlanoManutencaoCodigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadaPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    ModificadaPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    CustoEstimado = table.Column<decimal>(type: "numeric", nullable: true),
                    CustoPlanejado = table.Column<decimal>(type: "numeric", nullable: true),
                    CustoReal = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensManutencao", x => x.IdOrdem);
                    table.ForeignKey(
                        name: "FK_OrdensManutencao_NotasManutencao_NotaRelacionadaId",
                        column: x => x.NotaRelacionadaId,
                        principalTable: "NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdensManutencao_PlanosManutencaoNormalizados_PlanoManutenc~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalTable: "PlanosManutencaoNormalizados",
                        principalColumn: "CodigoPlano",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdensManutencao_UsuariosSap_CriadaPorUsuarioId",
                        column: x => x.CriadaPorUsuarioId,
                        principalTable: "UsuariosSap",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdensManutencao_UsuariosSap_ModificadaPorUsuarioId",
                        column: x => x.ModificadaPorUsuarioId,
                        principalTable: "UsuariosSap",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OperacoesOrdem",
                columns: table => new
                {
                    IdOperacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdemId = table.Column<long>(type: "bigint", nullable: false),
                    NomeOperacao = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    TipoAtividade = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DuracaoHoras = table.Column<decimal>(type: "numeric", nullable: true),
                    NumeroProfissionais = table.Column<int>(type: "integer", nullable: true),
                    TextoBreve = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DataInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperacoesOrdem", x => x.IdOperacao);
                    table.ForeignKey(
                        name: "FK_OperacoesOrdem_OrdensManutencao_OrdemId",
                        column: x => x.OrdemId,
                        principalTable: "OrdensManutencao",
                        principalColumn: "IdOrdem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotasCongas_Equipamento",
                table: "NotasCongas",
                column: "Equipamento");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosManutencao_IdEstrategia",
                table: "PlanosManutencao",
                column: "IdEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadesNota_NotaId",
                table: "AtividadesNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_CentrosTrabalho_Codigo",
                table: "CentrosTrabalho",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CicloComgasModel_IdEstrategia",
                table: "CicloComgasModel",
                column: "IdEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_CiclosManutencao_PlanoManutencaoCodigo",
                table: "CiclosManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_CentroTrabalhoId",
                table: "EquipamentosAtivos",
                column: "CentroTrabalhoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_EnderecoId",
                table: "EquipamentosAtivos",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_FabricanteId",
                table: "EquipamentosAtivos",
                column: "FabricanteId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_GpmId",
                table: "EquipamentosAtivos",
                column: "GpmId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_ImobilizadoTag",
                table: "EquipamentosAtivos",
                column: "ImobilizadoTag");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_LocalInstalacaoCodigo",
                table: "EquipamentosAtivos",
                column: "LocalInstalacaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosAtivos_LocalizacaoId",
                table: "EquipamentosAtivos",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fabricantes_Nome",
                table: "Fabricantes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GruposPlanejamentoManutencao_Codigo",
                table: "GruposPlanejamentoManutencao",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemManutencaoComgasModel_PlanoId",
                table: "ItemManutencaoComgasModel",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensNotaManutencao_NotaId",
                table: "ItensNotaManutencao",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPlanoManutencao_EquipamentoTag",
                table: "ItensPlanoManutencao",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPlanoManutencao_LocalInstalacaoCodigo",
                table: "ItensPlanoManutencao",
                column: "LocalInstalacaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPlanoManutencao_PlanoManutencaoCodigo",
                table: "ItensPlanoManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_LocaisInstalacao_CentroTrabalhoId",
                table: "LocaisInstalacao",
                column: "CentroTrabalhoId");

            migrationBuilder.CreateIndex(
                name: "IX_LocaisInstalacao_EnderecoId",
                table: "LocaisInstalacao",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_LocaisInstalacao_FabricanteId",
                table: "LocaisInstalacao",
                column: "FabricanteId");

            migrationBuilder.CreateIndex(
                name: "IX_LocaisInstalacao_GpmId",
                table: "LocaisInstalacao",
                column: "GpmId");

            migrationBuilder.CreateIndex(
                name: "IX_LocaisInstalacao_ImobilizadoTag",
                table: "LocaisInstalacao",
                column: "ImobilizadoTag");

            migrationBuilder.CreateIndex(
                name: "IX_LocaisInstalacao_LocalizacaoId",
                table: "LocaisInstalacao",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Localizacoes_CodigoLocalizacao",
                table: "Localizacoes",
                column: "CodigoLocalizacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localizacoes_EnderecoId",
                table: "Localizacoes",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_MedidasNota_NotaId",
                table: "MedidasNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasManutencao_CriadoPorUsuarioId",
                table: "NotasManutencao",
                column: "CriadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasManutencao_EquipamentoTag",
                table: "NotasManutencao",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_NotasManutencao_LocalInstalacaoCodigo",
                table: "NotasManutencao",
                column: "LocalInstalacaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_OperacoesOrdem_OrdemId",
                table: "OperacoesOrdem",
                column: "OrdemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensManutencao_CriadaPorUsuarioId",
                table: "OrdensManutencao",
                column: "CriadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensManutencao_ModificadaPorUsuarioId",
                table: "OrdensManutencao",
                column: "ModificadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensManutencao_NotaRelacionadaId",
                table: "OrdensManutencao",
                column: "NotaRelacionadaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensManutencao_PlanoManutencaoCodigo",
                table: "OrdensManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosManutencaoNormalizados_EstrategiaId",
                table: "PlanosManutencaoNormalizados",
                column: "EstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSap_Nome_Mandante",
                table: "UsuariosSap",
                columns: new[] { "Nome", "Mandante" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemNotaCongas_NotasCongas_NoteCongasIdNota",
                table: "ItemNotaCongas",
                column: "NoteCongasIdNota",
                principalTable: "NotasCongas",
                principalColumn: "IdNota",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotasCongas_Equipamentos_Equipamento",
                table: "NotasCongas",
                column: "Equipamento",
                principalTable: "Equipamentos",
                principalColumn: "IdEquipamento",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanosManutencao_EstrategiaComgasModel_IdEstrategia",
                table: "PlanosManutencao",
                column: "IdEstrategia",
                principalTable: "EstrategiaComgasModel",
                principalColumn: "IdEstrategia",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemNotaCongas_NotasCongas_NoteCongasIdNota",
                table: "ItemNotaCongas");

            migrationBuilder.DropForeignKey(
                name: "FK_NotasCongas_Equipamentos_Equipamento",
                table: "NotasCongas");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanosManutencao_EstrategiaComgasModel_IdEstrategia",
                table: "PlanosManutencao");

            migrationBuilder.DropTable(
                name: "AtividadesNota");

            migrationBuilder.DropTable(
                name: "CicloComgasModel");

            migrationBuilder.DropTable(
                name: "CiclosManutencao");

            migrationBuilder.DropTable(
                name: "ItemManutencaoComgasModel");

            migrationBuilder.DropTable(
                name: "ItensNotaManutencao");

            migrationBuilder.DropTable(
                name: "ItensPlanoManutencao");

            migrationBuilder.DropTable(
                name: "MedidasNota");

            migrationBuilder.DropTable(
                name: "OperacoesOrdem");

            migrationBuilder.DropTable(
                name: "EstrategiaComgasModel");

            migrationBuilder.DropTable(
                name: "OrdensManutencao");

            migrationBuilder.DropTable(
                name: "NotasManutencao");

            migrationBuilder.DropTable(
                name: "PlanosManutencaoNormalizados");

            migrationBuilder.DropTable(
                name: "EquipamentosAtivos");

            migrationBuilder.DropTable(
                name: "UsuariosSap");

            migrationBuilder.DropTable(
                name: "EstrategiasManutencao");

            migrationBuilder.DropTable(
                name: "LocaisInstalacao");

            migrationBuilder.DropTable(
                name: "CentrosTrabalho");

            migrationBuilder.DropTable(
                name: "Fabricantes");

            migrationBuilder.DropTable(
                name: "GruposPlanejamentoManutencao");

            migrationBuilder.DropTable(
                name: "Imobilizados");

            migrationBuilder.DropTable(
                name: "Localizacoes");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropIndex(
                name: "IX_NotasCongas_Equipamento",
                table: "NotasCongas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanosManutencao",
                table: "PlanosManutencao");

            migrationBuilder.DropIndex(
                name: "IX_PlanosManutencao_IdEstrategia",
                table: "PlanosManutencao");

            migrationBuilder.DropColumn(
                name: "DataPM",
                table: "PlanosManutencao");

            migrationBuilder.DropColumn(
                name: "IdEstrategia",
                table: "PlanosManutencao");

            migrationBuilder.RenameTable(
                name: "PlanosManutencao",
                newName: "PlanosManutnecao");

            migrationBuilder.AlterColumn<string>(
                name: "TipoNota",
                table: "NotasCongas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusUsuario",
                table: "NotasCongas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusSistema",
                table: "NotasCongas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentModelIdEquipamento",
                table: "NotasCongas",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unidade",
                table: "PlanosManutnecao",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "TxtCicloManut",
                table: "PlanosManutnecao",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemManut",
                table: "PlanosManutnecao",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanosManutnecao",
                table: "PlanosManutnecao",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCongas_EquipmentModelIdEquipamento",
                table: "NotasCongas",
                column: "EquipmentModelIdEquipamento");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemNotaCongas_NotasCongas_NoteCongasIdNota",
                table: "ItemNotaCongas",
                column: "NoteCongasIdNota",
                principalTable: "NotasCongas",
                principalColumn: "IdNota");

            migrationBuilder.AddForeignKey(
                name: "FK_NotasCongas_Equipamentos_EquipmentModelIdEquipamento",
                table: "NotasCongas",
                column: "EquipmentModelIdEquipamento",
                principalTable: "Equipamentos",
                principalColumn: "IdEquipamento");
        }
    }
}
