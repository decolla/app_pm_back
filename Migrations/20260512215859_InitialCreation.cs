using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "PM_Enderecos",
                schema: "public",
                columns: table => new
                {
                    IdEndereco = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rua = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Estado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Pais = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Enderecos", x => x.IdEndereco);
                });

            migrationBuilder.CreateTable(
                name: "PM_EstrategiasManutencao",
                schema: "public",
                columns: table => new
                {
                    IdEstrategia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Ciclos = table.Column<int>(type: "integer", nullable: false),
                    UnidadeCiclo = table.Column<int>(type: "integer", nullable: false),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    NumeroCiclos = table.Column<int>(type: "integer", nullable: false),
                    CicloContinuo = table.Column<bool>(type: "boolean", nullable: false),
                    TipoAtivoAplicavel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_EstrategiasManutencao", x => x.IdEstrategia);
                });

            migrationBuilder.CreateTable(
                name: "PM_Fabricantes",
                schema: "public",
                columns: table => new
                {
                    IdFabricante = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Fabricantes", x => x.IdFabricante);
                });

            migrationBuilder.CreateTable(
                name: "PM_Imobilizados",
                schema: "public",
                columns: table => new
                {
                    TagImobilizado = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    AtivoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    NomeImobilizado = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InformacaoAdicional = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PM_Imobilizados", x => x.TagImobilizado);
                });

            migrationBuilder.CreateTable(
                name: "PM_NotasPetro",
                schema: "public",
                columns: table => new
                {
                    IdNota = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Campo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    LocInstalacao = table.Column<string>(type: "text", nullable: false),
                    LocInstAfetado = table.Column<string>(type: "text", nullable: true),
                    AreaOp = table.Column<int>(type: "integer", nullable: false),
                    CenTrabPrinc = table.Column<string>(type: "text", nullable: false),
                    DataNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Localizacao = table.Column<string>(type: "text", nullable: false),
                    GrupoPlanej = table.Column<string>(type: "text", nullable: false),
                    TipoNota = table.Column<string>(type: "text", nullable: false),
                    Impacto = table.Column<int>(type: "integer", nullable: false),
                    Parada = table.Column<char>(type: "character(1)", nullable: true),
                    InicioIndisp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimIndisp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CondInstAntAvar = table.Column<int>(type: "integer", nullable: true),
                    CondInstDepAvar = table.Column<int>(type: "integer", nullable: true),
                    CondInstAposMed = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_NotasPetro", x => x.IdNota);
                });

            migrationBuilder.CreateTable(
                name: "PM_Regras",
                schema: "public",
                columns: table => new
                {
                    IdRegra = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalavraChave = table.Column<string>(type: "text", nullable: false),
                    CampoAlvo = table.Column<string>(type: "text", nullable: false),
                    ClassificacaoAplicada = table.Column<string>(type: "text", nullable: false),
                    OrdemPrioridade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Regras", x => x.IdRegra);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposManutencao",
                schema: "public",
                columns: table => new
                {
                    IdTipoManutencao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    TipoAtivo = table.Column<string>(type: "text", nullable: false),
                    Confiabilidade = table.Column<int>(type: "integer", nullable: false),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposManutencao", x => x.IdTipoManutencao);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposManutencaoCep",
                schema: "public",
                columns: table => new
                {
                    IdTipoManutencaoCep = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeTipo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    TipoAtivo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Confiabilidade = table.Column<int>(type: "integer", nullable: false),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    Niveis = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    AcoesFalha = table.Column<List<AcoesFalha>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposManutencaoCep", x => x.IdTipoManutencaoCep);
                });

            migrationBuilder.CreateTable(
                name: "PM_UsuariosSap",
                schema: "public",
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
                    table.PrimaryKey("PK_PM_UsuariosSap", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "PM_Localizacoes",
                schema: "public",
                columns: table => new
                {
                    IdLocalizacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CenManutencao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EmGarantia = table.Column<bool>(type: "boolean", nullable: true),
                    AreaOperacional = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Organizacao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CentroManutencao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CEP = table.Column<string>(type: "text", nullable: true),
                    CodigoAbc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CampoOrdenacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EnderecoId = table.Column<int>(type: "integer", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Rua = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Denominacao = table.Column<string>(type: "text", nullable: true),
                    DescricaoDenominacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Localizacoes", x => x.IdLocalizacao);
                    table.ForeignKey(
                        name: "FK_PM_Localizacoes_PM_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalSchema: "public",
                        principalTable: "PM_Enderecos",
                        principalColumn: "IdEndereco");
                });

            migrationBuilder.CreateTable(
                name: "PlanoManutencaoModel",
                schema: "public",
                columns: table => new
                {
                    CodigoPlano = table.Column<long>(type: "bigint", maxLength: 40, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomePlano = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NomeAtivo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EstrategiaId = table.Column<int>(type: "integer", nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GerarAutomaticamente = table.Column<int>(type: "integer", nullable: true),
                    Antencedencia = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UltimaOrdem = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoManutencaoModel", x => x.CodigoPlano);
                    table.ForeignKey(
                        name: "FK_PlanoManutencaoModel_PM_EstrategiasManutencao_EstrategiaId",
                        column: x => x.EstrategiaId,
                        principalSchema: "public",
                        principalTable: "PM_EstrategiasManutencao",
                        principalColumn: "IdEstrategia",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_OrdensPetro",
                schema: "public",
                columns: table => new
                {
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdNota = table.Column<int>(type: "integer", nullable: false),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ObservacoesEquipe = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_OrdensPetro", x => x.IdOrdem);
                    table.ForeignKey(
                        name: "FK_PM_OrdensPetro_PM_NotasPetro_IdNota",
                        column: x => x.IdNota,
                        principalSchema: "public",
                        principalTable: "PM_NotasPetro",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposEstrategia",
                schema: "public",
                columns: table => new
                {
                    IdTipoEstrategia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dias = table.Column<int>(type: "integer", nullable: false),
                    ToleranciaPositiva = table.Column<int>(type: "integer", nullable: false),
                    ToleranciaNegativa = table.Column<int>(type: "integer", nullable: false),
                    Continua = table.Column<bool>(type: "boolean", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    EstrategiaId = table.Column<int>(type: "integer", nullable: false),
                    IdTipoManutencaoCep = table.Column<int>(type: "integer", nullable: true),
                    IdTipoManutencao = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposEstrategia", x => x.IdTipoEstrategia);
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_EstrategiasManutencao_EstrategiaId",
                        column: x => x.EstrategiaId,
                        principalSchema: "public",
                        principalTable: "PM_EstrategiasManutencao",
                        principalColumn: "IdEstrategia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_TiposManutencaoCep_IdTipoManutencaoCep",
                        column: x => x.IdTipoManutencaoCep,
                        principalSchema: "public",
                        principalTable: "PM_TiposManutencaoCep",
                        principalColumn: "IdTipoManutencaoCep");
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalSchema: "public",
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_Almoxarifados",
                schema: "public",
                columns: table => new
                {
                    IdAlmoxarifado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Organizacao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    LocalizacaoId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Almoxarifados", x => x.IdAlmoxarifado);
                    table.ForeignKey(
                        name: "FK_PM_Almoxarifados_PM_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalSchema: "public",
                        principalTable: "PM_Localizacoes",
                        principalColumn: "IdLocalizacao");
                });

            migrationBuilder.CreateTable(
                name: "PM_CentrosPlanejamentoManutencao",
                schema: "public",
                columns: table => new
                {
                    IdCpm = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Organizacao = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Localizacao = table.Column<string>(type: "text", nullable: true),
                    LocalizacaoId = table.Column<int>(type: "integer", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_CentrosPlanejamentoManutencao", x => x.IdCpm);
                    table.ForeignKey(
                        name: "FK_PM_CentrosPlanejamentoManutencao_PM_Localizacoes_Localizaca~",
                        column: x => x.LocalizacaoId,
                        principalSchema: "public",
                        principalTable: "PM_Localizacoes",
                        principalColumn: "IdLocalizacao",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_SetoresVendas",
                schema: "public",
                columns: table => new
                {
                    IdSetor = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Organizacao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    LocalizacaoId = table.Column<int>(type: "integer", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_SetoresVendas", x => x.IdSetor);
                    table.ForeignKey(
                        name: "FK_PM_SetoresVendas_PM_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalSchema: "public",
                        principalTable: "PM_Localizacoes",
                        principalColumn: "IdLocalizacao");
                });

            migrationBuilder.CreateTable(
                name: "PM_CiclosManutencao",
                schema: "public",
                columns: table => new
                {
                    IdCiclo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Valor = table.Column<int>(type: "integer", nullable: false),
                    Unidade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NivelManutencao = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Offset = table.Column<int>(type: "integer", nullable: true),
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_CiclosManutencao", x => x.IdCiclo);
                    table.ForeignKey(
                        name: "FK_PM_CiclosManutencao_PlanoManutencaoModel_PlanoManutencaoCod~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalSchema: "public",
                        principalTable: "PlanoManutencaoModel",
                        principalColumn: "CodigoPlano",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposPlano",
                schema: "public",
                columns: table => new
                {
                    IdTipoManutencao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Visivel = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Dias = table.Column<int>(type: "integer", nullable: false),
                    ToleranciaPositiva = table.Column<int>(type: "integer", nullable: false),
                    ToleranciaNegativa = table.Column<int>(type: "integer", nullable: false),
                    Continua = table.Column<bool>(type: "boolean", nullable: false),
                    TipoEstrategiaId = table.Column<int>(type: "integer", nullable: true),
                    PlanoManutencaoId = table.Column<long>(type: "bigint", nullable: true),
                    TipoManutencaoCepId = table.Column<int>(type: "integer", nullable: true),
                    TipoManutencaoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposPlano", x => x.IdTipoManutencao);
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposEstrategia_TipoEstrategiaId",
                        column: x => x.TipoEstrategiaId,
                        principalSchema: "public",
                        principalTable: "PM_TiposEstrategia",
                        principalColumn: "IdTipoEstrategia");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposManutencaoCep_TipoManutencaoCepId",
                        column: x => x.TipoManutencaoCepId,
                        principalSchema: "public",
                        principalTable: "PM_TiposManutencaoCep",
                        principalColumn: "IdTipoManutencaoCep");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposManutencao_TipoManutencaoId",
                        column: x => x.TipoManutencaoId,
                        principalSchema: "public",
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PlanoManutencaoModel_PlanoManutencaoId",
                        column: x => x.PlanoManutencaoId,
                        principalSchema: "public",
                        principalTable: "PlanoManutencaoModel",
                        principalColumn: "CodigoPlano");
                });

            migrationBuilder.CreateTable(
                name: "PM_CentrosTrabalho",
                schema: "public",
                columns: table => new
                {
                    IdCentroTrabalho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CpmId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_CentrosTrabalho", x => x.IdCentroTrabalho);
                    table.ForeignKey(
                        name: "FK_PM_CentrosTrabalho_PM_CentrosPlanejamentoManutencao_CpmId",
                        column: x => x.CpmId,
                        principalSchema: "public",
                        principalTable: "PM_CentrosPlanejamentoManutencao",
                        principalColumn: "IdCpm",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_GruposPlanejamentoManutencao",
                schema: "public",
                columns: table => new
                {
                    IdGpm = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    IdCpm = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_GruposPlanejamentoManutencao", x => x.IdGpm);
                    table.ForeignKey(
                        name: "FK_PM_GruposPlanejamentoManutencao_PM_CentrosPlanejamentoManut~",
                        column: x => x.IdCpm,
                        principalSchema: "public",
                        principalTable: "PM_CentrosPlanejamentoManutencao",
                        principalColumn: "IdCpm",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_TarefasTipoManutencao",
                schema: "public",
                columns: table => new
                {
                    IdTarefa = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Duracao = table.Column<int>(type: "integer", nullable: false),
                    TipoManutencaoId = table.Column<int>(type: "integer", nullable: false),
                    TipoManutencaoModelIdTipoManutencao = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TarefasTipoManutencao", x => x.IdTarefa);
                    table.ForeignKey(
                        name: "FK_PM_TarefasTipoManutencao_PM_TiposManutencao_TipoManutencaoM~",
                        column: x => x.TipoManutencaoModelIdTipoManutencao,
                        principalSchema: "public",
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                    table.ForeignKey(
                        name: "FK_PM_TarefasTipoManutencao_PM_TiposPlano_TipoManutencaoId",
                        column: x => x.TipoManutencaoId,
                        principalSchema: "public",
                        principalTable: "PM_TiposPlano",
                        principalColumn: "IdTipoManutencao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Funcionarios",
                schema: "public",
                columns: table => new
                {
                    IdFuncionario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoFuncionario = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Funcao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Organizacao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    GpmId = table.Column<int>(type: "integer", nullable: false),
                    Permissao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Funcionarios", x => x.IdFuncionario);
                    table.ForeignKey(
                        name: "FK_PM_Funcionarios_PM_GruposPlanejamentoManutencao_GpmId",
                        column: x => x.GpmId,
                        principalSchema: "public",
                        principalTable: "PM_GruposPlanejamentoManutencao",
                        principalColumn: "IdGpm",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_LocaisInstalacao",
                schema: "public",
                columns: table => new
                {
                    CodigoLocalInstalacao = table.Column<int>(type: "integer", maxLength: 60, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_PM_LocaisInstalacao", x => x.CodigoLocalInstalacao);
                    table.ForeignKey(
                        name: "FK_PM_LocaisInstalacao_PM_CentrosTrabalho_CentroTrabalhoId",
                        column: x => x.CentroTrabalhoId,
                        principalSchema: "public",
                        principalTable: "PM_CentrosTrabalho",
                        principalColumn: "IdCentroTrabalho",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_LocaisInstalacao_PM_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalSchema: "public",
                        principalTable: "PM_Enderecos",
                        principalColumn: "IdEndereco",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_LocaisInstalacao_PM_Fabricantes_FabricanteId",
                        column: x => x.FabricanteId,
                        principalSchema: "public",
                        principalTable: "PM_Fabricantes",
                        principalColumn: "IdFabricante",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_LocaisInstalacao_PM_GruposPlanejamentoManutencao_GpmId",
                        column: x => x.GpmId,
                        principalSchema: "public",
                        principalTable: "PM_GruposPlanejamentoManutencao",
                        principalColumn: "IdGpm",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_LocaisInstalacao_PM_Imobilizados_ImobilizadoTag",
                        column: x => x.ImobilizadoTag,
                        principalSchema: "public",
                        principalTable: "PM_Imobilizados",
                        principalColumn: "TagImobilizado",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_LocaisInstalacao_PM_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalSchema: "public",
                        principalTable: "PM_Localizacoes",
                        principalColumn: "IdLocalizacao",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AlmoxarifadoFuncionario",
                schema: "public",
                columns: table => new
                {
                    AlmoxarifadoId = table.Column<int>(type: "integer", nullable: false),
                    FuncionarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlmoxarifadoFuncionario", x => new { x.AlmoxarifadoId, x.FuncionarioId });
                    table.ForeignKey(
                        name: "FK_AlmoxarifadoFuncionario_PM_Almoxarifados_AlmoxarifadoId",
                        column: x => x.AlmoxarifadoId,
                        principalSchema: "public",
                        principalTable: "PM_Almoxarifados",
                        principalColumn: "IdAlmoxarifado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlmoxarifadoFuncionario_PM_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalSchema: "public",
                        principalTable: "PM_Funcionarios",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CentroTrabalhoFuncionario",
                schema: "public",
                columns: table => new
                {
                    CentroTrabalhoId = table.Column<int>(type: "integer", nullable: false),
                    FuncionarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentroTrabalhoFuncionario", x => new { x.CentroTrabalhoId, x.FuncionarioId });
                    table.ForeignKey(
                        name: "FK_CentroTrabalhoFuncionario_PM_CentrosTrabalho_CentroTrabalho~",
                        column: x => x.CentroTrabalhoId,
                        principalSchema: "public",
                        principalTable: "PM_CentrosTrabalho",
                        principalColumn: "IdCentroTrabalho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CentroTrabalhoFuncionario_PM_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalSchema: "public",
                        principalTable: "PM_Funcionarios",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetorVendasFuncionario",
                schema: "public",
                columns: table => new
                {
                    SetorVendasId = table.Column<int>(type: "integer", nullable: false),
                    FuncionarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetorVendasFuncionario", x => new { x.SetorVendasId, x.FuncionarioId });
                    table.ForeignKey(
                        name: "FK_SetorVendasFuncionario_PM_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalSchema: "public",
                        principalTable: "PM_Funcionarios",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetorVendasFuncionario_PM_SetoresVendas_SetorVendasId",
                        column: x => x.SetorVendasId,
                        principalSchema: "public",
                        principalTable: "PM_SetoresVendas",
                        principalColumn: "IdSetor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_EquipamentosAtivos",
                schema: "public",
                columns: table => new
                {
                    TagEquipamento = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NumeroSerie = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    NumeroModelo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DenominacaoTipo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    AnoMesFabricacao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Denominacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    LocalInstalacaoCodigo = table.Column<int>(type: "integer", maxLength: 60, nullable: true),
                    DetalhesTecnicos = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_EquipamentosAtivos", x => x.TagEquipamento);
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_CentrosTrabalho_CentroTrabalhoId",
                        column: x => x.CentroTrabalhoId,
                        principalSchema: "public",
                        principalTable: "PM_CentrosTrabalho",
                        principalColumn: "IdCentroTrabalho",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalSchema: "public",
                        principalTable: "PM_Enderecos",
                        principalColumn: "IdEndereco");
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_Fabricantes_FabricanteId",
                        column: x => x.FabricanteId,
                        principalSchema: "public",
                        principalTable: "PM_Fabricantes",
                        principalColumn: "IdFabricante",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_GruposPlanejamentoManutencao_GpmId",
                        column: x => x.GpmId,
                        principalSchema: "public",
                        principalTable: "PM_GruposPlanejamentoManutencao",
                        principalColumn: "IdGpm");
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_Imobilizados_ImobilizadoTag",
                        column: x => x.ImobilizadoTag,
                        principalSchema: "public",
                        principalTable: "PM_Imobilizados",
                        principalColumn: "TagImobilizado",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_LocaisInstalacao_LocalInstalacaoCo~",
                        column: x => x.LocalInstalacaoCodigo,
                        principalSchema: "public",
                        principalTable: "PM_LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_EquipamentosAtivos_PM_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalSchema: "public",
                        principalTable: "PM_Localizacoes",
                        principalColumn: "IdLocalizacao",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_FerramentasTipo",
                schema: "public",
                columns: table => new
                {
                    IdFerramenta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoBreve = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TipoFerramenta = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    TextoDetalhado = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CentroTrabalhoId = table.Column<int>(type: "integer", nullable: true),
                    LocalInstalacaoCodigo = table.Column<int>(type: "integer", maxLength: 60, nullable: true),
                    FabricanteId = table.Column<int>(type: "integer", nullable: true),
                    PesoBruto = table.Column<decimal>(type: "numeric", nullable: true),
                    PesoBrutoUnidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PesoLiquido = table.Column<decimal>(type: "numeric", nullable: true),
                    PesoLiquidoUnidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Volume = table.Column<decimal>(type: "numeric", nullable: true),
                    VolumeUnidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Dimensao = table.Column<decimal>(type: "numeric", nullable: true),
                    DimensaoUnidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Valor = table.Column<decimal>(type: "numeric", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GarantiaAte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    QuantidadeDisp = table.Column<int>(type: "integer", nullable: false),
                    TipoManutencaoId = table.Column<int>(type: "integer", nullable: false),
                    TarefaModelIdTarefa = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_FerramentasTipo", x => x.IdFerramenta);
                    table.ForeignKey(
                        name: "FK_PM_FerramentasTipo_PM_CentrosTrabalho_CentroTrabalhoId",
                        column: x => x.CentroTrabalhoId,
                        principalSchema: "public",
                        principalTable: "PM_CentrosTrabalho",
                        principalColumn: "IdCentroTrabalho",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_FerramentasTipo_PM_Fabricantes_FabricanteId",
                        column: x => x.FabricanteId,
                        principalSchema: "public",
                        principalTable: "PM_Fabricantes",
                        principalColumn: "IdFabricante",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_FerramentasTipo_PM_LocaisInstalacao_LocalInstalacaoCodigo",
                        column: x => x.LocalInstalacaoCodigo,
                        principalSchema: "public",
                        principalTable: "PM_LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_FerramentasTipo_PM_TarefasTipoManutencao_TarefaModelIdTa~",
                        column: x => x.TarefaModelIdTarefa,
                        principalSchema: "public",
                        principalTable: "PM_TarefasTipoManutencao",
                        principalColumn: "IdTarefa");
                    table.ForeignKey(
                        name: "FK_PM_FerramentasTipo_PM_TiposPlano_TipoManutencaoId",
                        column: x => x.TipoManutencaoId,
                        principalSchema: "public",
                        principalTable: "PM_TiposPlano",
                        principalColumn: "IdTipoManutencao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_ComponentesEquipamento",
                schema: "public",
                columns: table => new
                {
                    IdComponente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DenominacaoEquipamento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CodigoComponente = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    NomeComponente = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: true),
                    UnidadeMedida = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ValidoDesde = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidoAte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InformacaoAdicional = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_ComponentesEquipamento", x => x.IdComponente);
                    table.ForeignKey(
                        name: "FK_PM_ComponentesEquipamento_PM_EquipamentosAtivos_Equipamento~",
                        column: x => x.EquipamentoTag,
                        principalSchema: "public",
                        principalTable: "PM_EquipamentosAtivos",
                        principalColumn: "TagEquipamento",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PM_ItensPlanoManutencao",
                schema: "public",
                columns: table => new
                {
                    IdItemPlano = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoItem = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", nullable: true),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    LocalInstalacaoCodigo = table.Column<int>(type: "integer", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_ItensPlanoManutencao", x => x.IdItemPlano);
                    table.ForeignKey(
                        name: "FK_PM_ItensPlanoManutencao_PM_EquipamentosAtivos_EquipamentoTag",
                        column: x => x.EquipamentoTag,
                        principalSchema: "public",
                        principalTable: "PM_EquipamentosAtivos",
                        principalColumn: "TagEquipamento",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_ItensPlanoManutencao_PM_LocaisInstalacao_LocalInstalacao~",
                        column: x => x.LocalInstalacaoCodigo,
                        principalSchema: "public",
                        principalTable: "PM_LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_ItensPlanoManutencao_PlanoManutencaoModel_PlanoManutenca~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalSchema: "public",
                        principalTable: "PlanoManutencaoModel",
                        principalColumn: "CodigoPlano",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_MatrizesLibra",
                schema: "public",
                columns: table => new
                {
                    IdMatrizLibra = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoLibra = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    ValidadeAte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_MatrizesLibra", x => x.IdMatrizLibra);
                    table.ForeignKey(
                        name: "FK_PM_MatrizesLibra_PM_EquipamentosAtivos_EquipamentoTag",
                        column: x => x.EquipamentoTag,
                        principalSchema: "public",
                        principalTable: "PM_EquipamentosAtivos",
                        principalColumn: "TagEquipamento");
                });

            migrationBuilder.CreateTable(
                name: "PM_NotasManutencao",
                schema: "public",
                columns: table => new
                {
                    IdNota = table.Column<long>(type: "bigint", nullable: false),
                    NumeroNota = table.Column<long>(type: "bigint", nullable: false),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DataNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CriadoPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    LocalInstalacaoCodigo = table.Column<int>(type: "integer", maxLength: 60, nullable: true),
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", nullable: true),
                    AreaOperacional = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ReferenciaLocalizacaoAtivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CategoriaNota = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Finalidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraAvaria = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EquipamentoDisponivel = table.Column<string>(type: "text", nullable: true),
                    AnaliseRiscoOperacional = table.Column<bool>(type: "boolean", nullable: true),
                    AnomaliaObservada = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ObservacoesComplementares = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PontoReferencia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MatrizLibra = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Temperatura = table.Column<decimal>(type: "numeric", nullable: true),
                    Tensao = table.Column<decimal>(type: "numeric", nullable: true),
                    Fluido = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Pressao = table.Column<decimal>(type: "numeric", nullable: true),
                    Plaqueta = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescricaoDetalhada = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    StatusNota = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: false),
                    InicioDesejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoNota = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoPrioridade = table.Column<string>(type: "text", nullable: true),
                    TipoDeNota = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodific = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCodif = table.Column<string>(type: "text", nullable: true),
                    Codificacao = table.Column<string>(type: "text", nullable: true),
                    Consequencia = table.Column<int>(type: "integer", nullable: true),
                    GrupoDeCodes = table.Column<string>(type: "text", nullable: true),
                    LocalInstalacaoModelCodigoLocalInstalacao = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_NotasManutencao", x => x.IdNota);
                    table.ForeignKey(
                        name: "FK_PM_NotasManutencao_PM_EquipamentosAtivos_EquipamentoTag",
                        column: x => x.EquipamentoTag,
                        principalSchema: "public",
                        principalTable: "PM_EquipamentosAtivos",
                        principalColumn: "TagEquipamento",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_NotasManutencao_PM_LocaisInstalacao_LocalInstalacaoModel~",
                        column: x => x.LocalInstalacaoModelCodigoLocalInstalacao,
                        principalSchema: "public",
                        principalTable: "PM_LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao");
                    table.ForeignKey(
                        name: "FK_PM_NotasManutencao_PM_UsuariosSap_CriadoPorUsuarioId",
                        column: x => x.CriadoPorUsuarioId,
                        principalSchema: "public",
                        principalTable: "PM_UsuariosSap",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_NotasManutencao_PlanoManutencaoModel_PlanoManutencaoCodi~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalSchema: "public",
                        principalTable: "PlanoManutencaoModel",
                        principalColumn: "CodigoPlano",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AlmoxarifadoFerramenta",
                schema: "public",
                columns: table => new
                {
                    AlmoxarifadoId = table.Column<int>(type: "integer", nullable: false),
                    FerramentaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlmoxarifadoFerramenta", x => new { x.AlmoxarifadoId, x.FerramentaId });
                    table.ForeignKey(
                        name: "FK_AlmoxarifadoFerramenta_PM_Almoxarifados_AlmoxarifadoId",
                        column: x => x.AlmoxarifadoId,
                        principalSchema: "public",
                        principalTable: "PM_Almoxarifados",
                        principalColumn: "IdAlmoxarifado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlmoxarifadoFerramenta_PM_FerramentasTipo_FerramentaId",
                        column: x => x.FerramentaId,
                        principalSchema: "public",
                        principalTable: "PM_FerramentasTipo",
                        principalColumn: "IdFerramenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_AtividadesNota",
                schema: "public",
                columns: table => new
                {
                    IdAtividade = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AtividadesNota", x => x.IdAtividade);
                    table.ForeignKey(
                        name: "FK_PM_AtividadesNota_PM_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalSchema: "public",
                        principalTable: "PM_NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_ItensNotaManutencao",
                schema: "public",
                columns: table => new
                {
                    IdItemNota = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ParteObjeto = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Sintoma = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoSintoma = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Causa = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoCausa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NDeOrdenacao1 = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodPartOb = table.Column<string>(type: "text", nullable: true),
                    TxtCodPartOb = table.Column<string>(type: "text", nullable: true),
                    TxtCodeProb = table.Column<string>(type: "text", nullable: true),
                    TextoGrpCausa = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodPr = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCausa = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_ItensNotaManutencao", x => x.IdItemNota);
                    table.ForeignKey(
                        name: "FK_PM_ItensNotaManutencao_PM_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalSchema: "public",
                        principalTable: "PM_NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_MedidasNota",
                schema: "public",
                columns: table => new
                {
                    IdMedida = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_MedidasNota", x => x.IdMedida);
                    table.ForeignKey(
                        name: "FK_PM_MedidasNota_PM_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalSchema: "public",
                        principalTable: "PM_NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_OrdensManutencao",
                schema: "public",
                columns: table => new
                {
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false),
                    TipoOrdem = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CodigoTrabalho = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DescricaoBreve = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Situacao = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    SituacaoPlanejamento = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusOrdem = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InicioPlanejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimPlanejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InicioReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    LocalInstalacaoCodigo = table.Column<int>(type: "integer", maxLength: 60, nullable: true),
                    AreaOperacional = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    GpmResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    Material = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    NumeroSerie = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DadosAdicionaisDispositivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Periodo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    NotaRelacionadaId = table.Column<long>(type: "bigint", nullable: true),
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", maxLength: 40, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadaPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    ModificadaPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    CustoEstimado = table.Column<decimal>(type: "numeric", nullable: true),
                    CustoPlanejado = table.Column<decimal>(type: "numeric", nullable: true),
                    CustoReal = table.Column<decimal>(type: "numeric", nullable: true),
                    EtiquetaNota = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AnaliseCriticidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    AnaliseRisco = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    AroAdtc = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    MatrizLibra = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    PermissaoTrabalho = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    LocalInstalacaoModelCodigoLocalInstalacao = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_OrdensManutencao", x => x.IdOrdem);
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PM_EquipamentosAtivos_EquipamentoTag",
                        column: x => x.EquipamentoTag,
                        principalSchema: "public",
                        principalTable: "PM_EquipamentosAtivos",
                        principalColumn: "TagEquipamento",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PM_GruposPlanejamentoManutencao_GpmResp~",
                        column: x => x.GpmResponsavelId,
                        principalSchema: "public",
                        principalTable: "PM_GruposPlanejamentoManutencao",
                        principalColumn: "IdGpm");
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PM_LocaisInstalacao_LocalInstalacaoMode~",
                        column: x => x.LocalInstalacaoModelCodigoLocalInstalacao,
                        principalSchema: "public",
                        principalTable: "PM_LocaisInstalacao",
                        principalColumn: "CodigoLocalInstalacao");
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PM_NotasManutencao_NotaRelacionadaId",
                        column: x => x.NotaRelacionadaId,
                        principalSchema: "public",
                        principalTable: "PM_NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PM_UsuariosSap_CriadaPorUsuarioId",
                        column: x => x.CriadaPorUsuarioId,
                        principalSchema: "public",
                        principalTable: "PM_UsuariosSap",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PM_UsuariosSap_ModificadaPorUsuarioId",
                        column: x => x.ModificadaPorUsuarioId,
                        principalSchema: "public",
                        principalTable: "PM_UsuariosSap",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PM_OrdensManutencao_PlanoManutencaoModel_PlanoManutencaoCod~",
                        column: x => x.PlanoManutencaoCodigo,
                        principalSchema: "public",
                        principalTable: "PlanoManutencaoModel",
                        principalColumn: "CodigoPlano");
                });

            migrationBuilder.CreateTable(
                name: "PM_VariaveisNota",
                schema: "public",
                columns: table => new
                {
                    IdVariavel = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ValorNumerico = table.Column<decimal>(type: "numeric", nullable: true),
                    UnidadeMedida = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Comentarios = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_VariaveisNota", x => x.IdVariavel);
                    table.ForeignKey(
                        name: "FK_PM_VariaveisNota_PM_NotasManutencao_NotaId",
                        column: x => x.NotaId,
                        principalSchema: "public",
                        principalTable: "PM_NotasManutencao",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_AnalisesRisco",
                schema: "public",
                columns: table => new
                {
                    IdAnaliseRisco = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoAnaliseRisco = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ResultadoAnalise = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    TagOrdem = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    OrdemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AnalisesRisco", x => x.IdAnaliseRisco);
                    table.ForeignKey(
                        name: "FK_PM_AnalisesRisco_PM_OrdensManutencao_OrdemId",
                        column: x => x.OrdemId,
                        principalSchema: "public",
                        principalTable: "PM_OrdensManutencao",
                        principalColumn: "IdOrdem");
                });

            migrationBuilder.CreateTable(
                name: "PM_AnalisesRiscoOperacional",
                schema: "public",
                columns: table => new
                {
                    IdAnaliseRiscoOperacional = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoAro = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ResultadoAnalise = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    TagOrdem = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    OrdemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AnalisesRiscoOperacional", x => x.IdAnaliseRiscoOperacional);
                    table.ForeignKey(
                        name: "FK_PM_AnalisesRiscoOperacional_PM_OrdensManutencao_OrdemId",
                        column: x => x.OrdemId,
                        principalSchema: "public",
                        principalTable: "PM_OrdensManutencao",
                        principalColumn: "IdOrdem");
                });

            migrationBuilder.CreateTable(
                name: "PM_OperacoesOrdem",
                schema: "public",
                columns: table => new
                {
                    IdOperacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdemId = table.Column<long>(type: "bigint", nullable: false),
                    NomeOperacao = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DuracaoHoras = table.Column<decimal>(type: "numeric", nullable: true),
                    NumeroProfissionais = table.Column<int>(type: "integer", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoAtividade = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoBreve = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TextoDetalhado = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_OperacoesOrdem", x => x.IdOperacao);
                    table.ForeignKey(
                        name: "FK_PM_OperacoesOrdem_PM_OrdensManutencao_OrdemId",
                        column: x => x.OrdemId,
                        principalSchema: "public",
                        principalTable: "PM_OrdensManutencao",
                        principalColumn: "IdOrdem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_PermissoesTrabalho",
                schema: "public",
                columns: table => new
                {
                    IdPermissaoTrabalho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoPt = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PermissaoConcedida = table.Column<bool>(type: "boolean", nullable: true),
                    TagOrdem = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    OrdemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_PermissoesTrabalho", x => x.IdPermissaoTrabalho);
                    table.ForeignKey(
                        name: "FK_PM_PermissoesTrabalho_PM_OrdensManutencao_OrdemId",
                        column: x => x.OrdemId,
                        principalSchema: "public",
                        principalTable: "PM_OrdensManutencao",
                        principalColumn: "IdOrdem");
                });

            migrationBuilder.CreateTable(
                name: "OperacaoOrdemFerramenta",
                schema: "public",
                columns: table => new
                {
                    OperacaoId = table.Column<int>(type: "integer", nullable: false),
                    FerramentaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperacaoOrdemFerramenta", x => new { x.OperacaoId, x.FerramentaId });
                    table.ForeignKey(
                        name: "FK_OperacaoOrdemFerramenta_PM_FerramentasTipo_FerramentaId",
                        column: x => x.FerramentaId,
                        principalSchema: "public",
                        principalTable: "PM_FerramentasTipo",
                        principalColumn: "IdFerramenta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperacaoOrdemFerramenta_PM_OperacoesOrdem_OperacaoId",
                        column: x => x.OperacaoId,
                        principalSchema: "public",
                        principalTable: "PM_OperacoesOrdem",
                        principalColumn: "IdOperacao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Anexos",
                schema: "public",
                columns: table => new
                {
                    IdAnexo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeAnexo = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: false),
                    Dados = table.Column<byte[]>(type: "bytea", nullable: false),
                    ReferenciaId = table.Column<long>(type: "bigint", nullable: false),
                    TipoReferencia = table.Column<string>(type: "text", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AnaliseRiscoModelIdAnaliseRisco = table.Column<int>(type: "integer", nullable: true),
                    AnaliseRiscoOperacionalModelIdAnaliseRiscoOperacional = table.Column<int>(type: "integer", nullable: true),
                    MatrizLibraModelIdMatrizLibra = table.Column<int>(type: "integer", nullable: true),
                    NotaManutencaoModelIdNota = table.Column<long>(type: "bigint", nullable: true),
                    PermissaoTrabalhoModelIdPermissaoTrabalho = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Anexos", x => x.IdAnexo);
                    table.ForeignKey(
                        name: "FK_PM_Anexos_PM_AnalisesRiscoOperacional_AnaliseRiscoOperacion~",
                        column: x => x.AnaliseRiscoOperacionalModelIdAnaliseRiscoOperacional,
                        principalSchema: "public",
                        principalTable: "PM_AnalisesRiscoOperacional",
                        principalColumn: "IdAnaliseRiscoOperacional");
                    table.ForeignKey(
                        name: "FK_PM_Anexos_PM_AnalisesRisco_AnaliseRiscoModelIdAnaliseRisco",
                        column: x => x.AnaliseRiscoModelIdAnaliseRisco,
                        principalSchema: "public",
                        principalTable: "PM_AnalisesRisco",
                        principalColumn: "IdAnaliseRisco");
                    table.ForeignKey(
                        name: "FK_PM_Anexos_PM_MatrizesLibra_MatrizLibraModelIdMatrizLibra",
                        column: x => x.MatrizLibraModelIdMatrizLibra,
                        principalSchema: "public",
                        principalTable: "PM_MatrizesLibra",
                        principalColumn: "IdMatrizLibra");
                    table.ForeignKey(
                        name: "FK_PM_Anexos_PM_NotasManutencao_NotaManutencaoModelIdNota",
                        column: x => x.NotaManutencaoModelIdNota,
                        principalSchema: "public",
                        principalTable: "PM_NotasManutencao",
                        principalColumn: "IdNota");
                    table.ForeignKey(
                        name: "FK_PM_Anexos_PM_PermissoesTrabalho_PermissaoTrabalhoModelIdPer~",
                        column: x => x.PermissaoTrabalhoModelIdPermissaoTrabalho,
                        principalSchema: "public",
                        principalTable: "PM_PermissoesTrabalho",
                        principalColumn: "IdPermissaoTrabalho");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlmoxarifadoFerramenta_FerramentaId",
                schema: "public",
                table: "AlmoxarifadoFerramenta",
                column: "FerramentaId");

            migrationBuilder.CreateIndex(
                name: "IX_AlmoxarifadoFuncionario_FuncionarioId",
                schema: "public",
                table: "AlmoxarifadoFuncionario",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CentroTrabalhoFuncionario_FuncionarioId",
                schema: "public",
                table: "CentroTrabalhoFuncionario",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OperacaoOrdemFerramenta_FerramentaId",
                schema: "public",
                table: "OperacaoOrdemFerramenta",
                column: "FerramentaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoManutencaoModel_EstrategiaId",
                schema: "public",
                table: "PlanoManutencaoModel",
                column: "EstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Almoxarifados_LocalizacaoId",
                schema: "public",
                table: "PM_Almoxarifados",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AnalisesRisco_OrdemId",
                schema: "public",
                table: "PM_AnalisesRisco",
                column: "OrdemId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AnalisesRiscoOperacional_OrdemId",
                schema: "public",
                table: "PM_AnalisesRiscoOperacional",
                column: "OrdemId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Anexos_AnaliseRiscoModelIdAnaliseRisco",
                schema: "public",
                table: "PM_Anexos",
                column: "AnaliseRiscoModelIdAnaliseRisco");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Anexos_AnaliseRiscoOperacionalModelIdAnaliseRiscoOperaci~",
                schema: "public",
                table: "PM_Anexos",
                column: "AnaliseRiscoOperacionalModelIdAnaliseRiscoOperacional");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Anexos_MatrizLibraModelIdMatrizLibra",
                schema: "public",
                table: "PM_Anexos",
                column: "MatrizLibraModelIdMatrizLibra");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Anexos_NotaManutencaoModelIdNota",
                schema: "public",
                table: "PM_Anexos",
                column: "NotaManutencaoModelIdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Anexos_PermissaoTrabalhoModelIdPermissaoTrabalho",
                schema: "public",
                table: "PM_Anexos",
                column: "PermissaoTrabalhoModelIdPermissaoTrabalho");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AtividadesNota_NotaId",
                schema: "public",
                table: "PM_AtividadesNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_CentrosPlanejamentoManutencao_LocalizacaoId",
                schema: "public",
                table: "PM_CentrosPlanejamentoManutencao",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_CentrosTrabalho_Codigo",
                schema: "public",
                table: "PM_CentrosTrabalho",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_CentrosTrabalho_CpmId",
                schema: "public",
                table: "PM_CentrosTrabalho",
                column: "CpmId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_CiclosManutencao_PlanoManutencaoCodigo",
                schema: "public",
                table: "PM_CiclosManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_ComponentesEquipamento_EquipamentoTag",
                schema: "public",
                table: "PM_ComponentesEquipamento",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_CentroTrabalhoId",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "CentroTrabalhoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_EnderecoId",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_FabricanteId",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "FabricanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_GpmId",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "GpmId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_ImobilizadoTag",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "ImobilizadoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_LocalInstalacaoCodigo",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "LocalInstalacaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_EquipamentosAtivos_LocalizacaoId",
                schema: "public",
                table: "PM_EquipamentosAtivos",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Fabricantes_Nome",
                schema: "public",
                table: "PM_Fabricantes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasTipo_CentroTrabalhoId",
                schema: "public",
                table: "PM_FerramentasTipo",
                column: "CentroTrabalhoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasTipo_FabricanteId",
                schema: "public",
                table: "PM_FerramentasTipo",
                column: "FabricanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasTipo_LocalInstalacaoCodigo",
                schema: "public",
                table: "PM_FerramentasTipo",
                column: "LocalInstalacaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasTipo_TarefaModelIdTarefa",
                schema: "public",
                table: "PM_FerramentasTipo",
                column: "TarefaModelIdTarefa");

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasTipo_TipoManutencaoId",
                schema: "public",
                table: "PM_FerramentasTipo",
                column: "TipoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Funcionarios_Cpf",
                schema: "public",
                table: "PM_Funcionarios",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_Funcionarios_GpmId",
                schema: "public",
                table: "PM_Funcionarios",
                column: "GpmId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_GruposPlanejamentoManutencao_IdCpm",
                schema: "public",
                table: "PM_GruposPlanejamentoManutencao",
                column: "IdCpm");

            migrationBuilder.CreateIndex(
                name: "IX_PM_GruposPlanejamentoManutencao_IdGpm",
                schema: "public",
                table: "PM_GruposPlanejamentoManutencao",
                column: "IdGpm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_ItensNotaManutencao_NotaId",
                schema: "public",
                table: "PM_ItensNotaManutencao",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_ItensPlanoManutencao_EquipamentoTag",
                schema: "public",
                table: "PM_ItensPlanoManutencao",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_ItensPlanoManutencao_LocalInstalacaoCodigo",
                schema: "public",
                table: "PM_ItensPlanoManutencao",
                column: "LocalInstalacaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_ItensPlanoManutencao_PlanoManutencaoCodigo",
                schema: "public",
                table: "PM_ItensPlanoManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_LocaisInstalacao_CentroTrabalhoId",
                schema: "public",
                table: "PM_LocaisInstalacao",
                column: "CentroTrabalhoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_LocaisInstalacao_EnderecoId",
                schema: "public",
                table: "PM_LocaisInstalacao",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_LocaisInstalacao_FabricanteId",
                schema: "public",
                table: "PM_LocaisInstalacao",
                column: "FabricanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_LocaisInstalacao_GpmId",
                schema: "public",
                table: "PM_LocaisInstalacao",
                column: "GpmId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_LocaisInstalacao_ImobilizadoTag",
                schema: "public",
                table: "PM_LocaisInstalacao",
                column: "ImobilizadoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_LocaisInstalacao_LocalizacaoId",
                schema: "public",
                table: "PM_LocaisInstalacao",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Localizacoes_EnderecoId",
                schema: "public",
                table: "PM_Localizacoes",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Localizacoes_IdLocalizacao",
                schema: "public",
                table: "PM_Localizacoes",
                column: "IdLocalizacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_MatrizesLibra_EquipamentoTag",
                schema: "public",
                table: "PM_MatrizesLibra",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_MedidasNota_NotaId",
                schema: "public",
                table: "PM_MedidasNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_NotasManutencao_CriadoPorUsuarioId",
                schema: "public",
                table: "PM_NotasManutencao",
                column: "CriadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_NotasManutencao_EquipamentoTag",
                schema: "public",
                table: "PM_NotasManutencao",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_NotasManutencao_LocalInstalacaoModelCodigoLocalInstalacao",
                schema: "public",
                table: "PM_NotasManutencao",
                column: "LocalInstalacaoModelCodigoLocalInstalacao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_NotasManutencao_NumeroNota",
                schema: "public",
                table: "PM_NotasManutencao",
                column: "NumeroNota",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_NotasManutencao_PlanoManutencaoCodigo",
                schema: "public",
                table: "PM_NotasManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OperacoesOrdem_OrdemId",
                schema: "public",
                table: "PM_OperacoesOrdem",
                column: "OrdemId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_CriadaPorUsuarioId",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "CriadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_EquipamentoTag",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "EquipamentoTag");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_GpmResponsavelId",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "GpmResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_LocalInstalacaoModelCodigoLocalInstalac~",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "LocalInstalacaoModelCodigoLocalInstalacao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_ModificadaPorUsuarioId",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "ModificadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_NotaRelacionadaId",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "NotaRelacionadaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensManutencao_PlanoManutencaoCodigo",
                schema: "public",
                table: "PM_OrdensManutencao",
                column: "PlanoManutencaoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PM_OrdensPetro_IdNota",
                schema: "public",
                table: "PM_OrdensPetro",
                column: "IdNota",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_PermissoesTrabalho_OrdemId",
                schema: "public",
                table: "PM_PermissoesTrabalho",
                column: "OrdemId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_SetoresVendas_LocalizacaoId",
                schema: "public",
                table: "PM_SetoresVendas",
                column: "LocalizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TarefasTipoManutencao_TipoManutencaoId",
                schema: "public",
                table: "PM_TarefasTipoManutencao",
                column: "TipoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TarefasTipoManutencao_TipoManutencaoModelIdTipoManutencao",
                schema: "public",
                table: "PM_TarefasTipoManutencao",
                column: "TipoManutencaoModelIdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_EstrategiaId",
                schema: "public",
                table: "PM_TiposEstrategia",
                column: "EstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdTipoManutencao",
                schema: "public",
                table: "PM_TiposEstrategia",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdTipoManutencaoCep",
                schema: "public",
                table: "PM_TiposEstrategia",
                column: "IdTipoManutencaoCep");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposManutencaoCep_NomeTipo_TipoAtivo",
                schema: "public",
                table: "PM_TiposManutencaoCep",
                columns: new[] { "NomeTipo", "TipoAtivo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_PlanoManutencaoId",
                schema: "public",
                table: "PM_TiposPlano",
                column: "PlanoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_TipoEstrategiaId",
                schema: "public",
                table: "PM_TiposPlano",
                column: "TipoEstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_TipoManutencaoCepId",
                schema: "public",
                table: "PM_TiposPlano",
                column: "TipoManutencaoCepId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_TipoManutencaoId",
                schema: "public",
                table: "PM_TiposPlano",
                column: "TipoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PM_UsuariosSap_Nome_Mandante",
                schema: "public",
                table: "PM_UsuariosSap",
                columns: new[] { "Nome", "Mandante" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PM_VariaveisNota_NotaId",
                schema: "public",
                table: "PM_VariaveisNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_SetorVendasFuncionario_FuncionarioId",
                schema: "public",
                table: "SetorVendasFuncionario",
                column: "FuncionarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlmoxarifadoFerramenta",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AlmoxarifadoFuncionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CentroTrabalhoFuncionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OperacaoOrdemFerramenta",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Anexos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_AtividadesNota",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_CiclosManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_ComponentesEquipamento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_ItensNotaManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_ItensPlanoManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_MedidasNota",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_OrdensPetro",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Regras",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_VariaveisNota",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SetorVendasFuncionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Almoxarifados",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_FerramentasTipo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_OperacoesOrdem",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_AnalisesRiscoOperacional",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_AnalisesRisco",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_MatrizesLibra",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_PermissoesTrabalho",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_NotasPetro",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Funcionarios",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_SetoresVendas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_TarefasTipoManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_OrdensManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_TiposPlano",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_NotasManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_TiposEstrategia",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_EquipamentosAtivos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_UsuariosSap",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PlanoManutencaoModel",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_TiposManutencaoCep",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_TiposManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_LocaisInstalacao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_EstrategiasManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_CentrosTrabalho",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Fabricantes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_GruposPlanejamentoManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Imobilizados",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_CentrosPlanejamentoManutencao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Localizacoes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PM_Enderecos",
                schema: "public");
        }
    }
}
