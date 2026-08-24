using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using app_pm_back.Models.Domain;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estrategias",
                columns: table => new
                {
                    IdEstrategia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Ciclos = table.Column<int>(type: "integer", nullable: true),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    TipoLi = table.Column<int>(type: "integer", nullable: true),
                    NumeroCiclos = table.Column<int>(type: "integer", nullable: true),
                    CicloContinuo = table.Column<bool>(type: "boolean", nullable: true),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estrategias", x => x.IdEstrategia);
                });

            migrationBuilder.CreateTable(
                name: "PM_Almoxarifados",
                columns: table => new
                {
                    IdAlmoxarifado = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Organizacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Almoxarifados", x => x.IdAlmoxarifado);
                });

            migrationBuilder.CreateTable(
                name: "PM_Auditorias",
                columns: table => new
                {
                    IdAuditoria = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NomeTabela = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Acao = table.Column<int>(type: "integer", maxLength: 30, nullable: false),
                    ChavePrimaria = table.Column<string>(type: "text", nullable: false),
                    ValoresAntigos = table.Column<string>(type: "text", nullable: true),
                    ValoresNovos = table.Column<string>(type: "text", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Auditorias", x => x.IdAuditoria);
                });

            migrationBuilder.CreateTable(
                name: "PM_ConfiguracoesGlobais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FichasConfiabilidade = table.Column<int>(type: "integer", nullable: false),
                    ClassificacaoAutomatica = table.Column<int>(type: "integer", nullable: false),
                    CriterioDataRenovacao = table.Column<int>(type: "integer", nullable: false),
                    CriterioConfiabilidadeRenovacao = table.Column<int>(type: "integer", nullable: false),
                    CriarFichasPreventivas = table.Column<int>(type: "integer", nullable: false),
                    DataModificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModificadoPorUsuario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_ConfiguracoesGlobais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PM_Cpms",
                columns: table => new
                {
                    IdCPM = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeCentro = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Organizacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Cpms", x => x.IdCPM);
                });

            migrationBuilder.CreateTable(
                name: "PM_Diretorios",
                columns: table => new
                {
                    IdDiretorio = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdDiretorioPai = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Diretorios", x => x.IdDiretorio);
                    table.ForeignKey(
                        name: "FK_PM_Diretorios_PM_Diretorios_IdDiretorioPai",
                        column: x => x.IdDiretorioPai,
                        principalTable: "PM_Diretorios",
                        principalColumn: "IdDiretorio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PM_Imobilizados",
                columns: table => new
                {
                    IdImobilizado = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Nserie = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataIncorporacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValorCompra = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Moeda = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Depreciacao = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ValorAtual = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    InfoAdicionais = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IdAtivo = table.Column<int>(type: "integer", nullable: true),
                    IdLi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Imobilizados", x => x.IdImobilizado);
                });

            migrationBuilder.CreateTable(
                name: "PM_Reliability",
                columns: table => new
                {
                    IdLi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdsNotas = table.Column<List<long>>(type: "bigint[]", nullable: true),
                    IdsOrdens = table.Column<List<long>>(type: "bigint[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Reliability", x => x.IdLi);
                });

            migrationBuilder.CreateTable(
                name: "PM_RuleClassification",
                columns: table => new
                {
                    IdRegra = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalavraChave = table.Column<string>(type: "text", nullable: false),
                    ClassificacaoAplicada = table.Column<int>(type: "integer", nullable: false),
                    Permitida = table.Column<bool>(type: "boolean", nullable: false),
                    OrdemPrioridade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_RuleClassification", x => x.IdRegra);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposCep",
                columns: table => new
                {
                    IdTipoCep = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoAtivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Confiabilidade = table.Column<double>(type: "double precision", nullable: true),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: true),
                    CampoIndefinido = table.Column<string>(type: "text", nullable: true),
                    AcoesFalha = table.Column<List<AcoesFalha>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposCep", x => x.IdTipoCep);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposManutencao",
                columns: table => new
                {
                    IdTipoManutencao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categoria = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TipoLI = table.Column<int>(type: "integer", nullable: false),
                    Confiabilidade = table.Column<double>(type: "double precision", nullable: true),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposManutencao", x => x.IdTipoManutencao);
                });

            migrationBuilder.CreateTable(
                name: "PM_Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CPF = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Permissoes = table.Column<int>(type: "integer", nullable: true),
                    NivelPermissao = table.Column<int>(type: "integer", nullable: true),
                    Escopos = table.Column<List<long>>(type: "bigint[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "RegraClassificacaoDto",
                columns: table => new
                {
                    IdRegra = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalavraChave = table.Column<string>(type: "text", nullable: false),
                    ClassificacaoAplicada = table.Column<int>(type: "integer", nullable: false),
                    Permitida = table.Column<bool>(type: "boolean", nullable: false),
                    OrdemPrioridade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegraClassificacaoDto", x => x.IdRegra);
                });

            migrationBuilder.CreateTable(
                name: "PM_Planos",
                columns: table => new
                {
                    IdPlano = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    DataInicios = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GerarAutomaticamente = table.Column<int>(type: "integer", nullable: true),
                    Antecedencia = table.Column<int>(type: "integer", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IdAtivo = table.Column<int>(type: "integer", nullable: true),
                    IdLi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IdEstrategia = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Planos", x => x.IdPlano);
                    table.ForeignKey(
                        name: "FK_PM_Planos_Estrategias_IdEstrategia",
                        column: x => x.IdEstrategia,
                        principalTable: "Estrategias",
                        principalColumn: "IdEstrategia");
                });

            migrationBuilder.CreateTable(
                name: "PM_Ferramentas",
                columns: table => new
                {
                    IdFerramenta = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    TipoFerramenta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PesoBruto = table.Column<double>(type: "double precision", nullable: false),
                    PesoLiquido = table.Column<double>(type: "double precision", nullable: false),
                    UmPesoBruto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UmPesoLiquido = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Volume = table.Column<double>(type: "double precision", nullable: false),
                    UmVolume = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Dimensao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UmDimensao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Fabricante = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ValorCompra = table.Column<decimal>(type: "numeric", nullable: false),
                    Moeda = table.Column<string>(type: "text", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IdLI = table.Column<string>(type: "text", nullable: false),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: false),
                    IdAlmoxarifado = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Ferramentas", x => x.IdFerramenta);
                    table.ForeignKey(
                        name: "FK_PM_Ferramentas_PM_Almoxarifados_IdAlmoxarifado",
                        column: x => x.IdAlmoxarifado,
                        principalTable: "PM_Almoxarifados",
                        principalColumn: "IdAlmoxarifado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Gpms",
                columns: table => new
                {
                    IdGPM = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IdCPM = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Gpms", x => x.IdGPM);
                    table.ForeignKey(
                        name: "FK_PM_Gpms_PM_Cpms_IdCPM",
                        column: x => x.IdCPM,
                        principalTable: "PM_Cpms",
                        principalColumn: "IdCPM");
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposEstrategia",
                columns: table => new
                {
                    IdTipoEstrategia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dias = table.Column<int>(type: "integer", nullable: true),
                    ToleranciaPositiva = table.Column<double>(type: "double precision", nullable: true),
                    ToleranciaNegativa = table.Column<double>(type: "double precision", nullable: true),
                    Continua = table.Column<bool>(type: "boolean", nullable: true),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    IdEstrategia = table.Column<long>(type: "bigint", nullable: true),
                    IdTipoManutencao = table.Column<long>(type: "bigint", nullable: true),
                    IdTipoCep = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposEstrategia", x => x.IdTipoEstrategia);
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_Estrategias_IdEstrategia",
                        column: x => x.IdEstrategia,
                        principalTable: "Estrategias",
                        principalColumn: "IdEstrategia");
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_TiposCep_IdTipoCep",
                        column: x => x.IdTipoCep,
                        principalTable: "PM_TiposCep",
                        principalColumn: "IdTipoCep");
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_AlmoxarifadoUsuario",
                columns: table => new
                {
                    IdAlmoxarifado = table.Column<long>(type: "bigint", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AlmoxarifadoUsuario", x => new { x.IdAlmoxarifado, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_PM_AlmoxarifadoUsuario_PM_Almoxarifados_IdAlmoxarifado",
                        column: x => x.IdAlmoxarifado,
                        principalTable: "PM_Almoxarifados",
                        principalColumn: "IdAlmoxarifado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_AlmoxarifadoUsuario_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Ctms",
                columns: table => new
                {
                    IdCTM = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AreaCTM = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IdCPM = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Ctms", x => x.IdCTM);
                    table.ForeignKey(
                        name: "FK_PM_Ctms_PM_Cpms_IdCPM",
                        column: x => x.IdCPM,
                        principalTable: "PM_Cpms",
                        principalColumn: "IdCPM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_Ctms_PM_Gpms_IdCTM",
                        column: x => x.IdCTM,
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_GpmLi",
                columns: table => new
                {
                    IdGPM = table.Column<long>(type: "bigint", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_GpmLi", x => new { x.IdGPM, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_PM_GpmLi_PM_Gpms_IdGPM",
                        column: x => x.IdGPM,
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_GpmLi_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_GpmUsuario",
                columns: table => new
                {
                    IdGPM = table.Column<long>(type: "bigint", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_GpmUsuario", x => new { x.IdGPM, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_PM_GpmUsuario_PM_Gpms_IdGPM",
                        column: x => x.IdGPM,
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_GpmUsuario_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Notas",
                columns: table => new
                {
                    IdNota = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SapCode = table.Column<string>(type: "text", nullable: true),
                    NumeroNota = table.Column<long>(type: "bigint", nullable: false),
                    DataNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoNota = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenciaLocalizacaoAtivo = table.Column<string>(type: "text", nullable: true),
                    Finalidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    FimDesejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusNota = table.Column<int>(type: "integer", nullable: true),
                    EquipamentoDisponivel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConfiabilidadeRestaurada = table.Column<int>(type: "integer", nullable: true),
                    DescricaoBreve = table.Column<string>(type: "text", nullable: true),
                    DescricaoLonga = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AnomaliaObservada = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ObservacoesComplementares = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Classificacao = table.Column<int>(type: "integer", nullable: false),
                    MetodoClassificacao = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    ClassificadoPor = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadaPorUsuario = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ModificadaPorUsuario = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DataModificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Escopo = table.Column<long>(type: "bigint", nullable: true),
                    IdPlano = table.Column<long>(type: "bigint", nullable: true),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: true),
                    IdAtivo = table.Column<int>(type: "integer", nullable: true),
                    IdLi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IdGpm = table.Column<long>(type: "bigint", nullable: true),
                    DnsIot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Notas", x => x.IdNota);
                    table.ForeignKey(
                        name: "FK_PM_Notas_PM_Gpms_IdGpm",
                        column: x => x.IdGpm,
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM");
                    table.ForeignKey(
                        name: "FK_PM_Notas_PM_Planos_IdPlano",
                        column: x => x.IdPlano,
                        principalTable: "PM_Planos",
                        principalColumn: "IdPlano");
                    table.ForeignKey(
                        name: "FK_PM_Notas_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "PM_CtmUsuario",
                columns: table => new
                {
                    IdCTM = table.Column<long>(type: "bigint", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_CtmUsuario", x => new { x.IdCTM, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_PM_CtmUsuario_PM_Ctms_IdCTM",
                        column: x => x.IdCTM,
                        principalTable: "PM_Ctms",
                        principalColumn: "IdCTM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_CtmUsuario_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiretorioNotaModel",
                columns: table => new
                {
                    IdDiretorio = table.Column<long>(type: "bigint", nullable: false),
                    IdNota = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiretorioNotaModel", x => new { x.IdDiretorio, x.IdNota });
                    table.ForeignKey(
                        name: "FK_DiretorioNotaModel_PM_Diretorios_IdDiretorio",
                        column: x => x.IdDiretorio,
                        principalTable: "PM_Diretorios",
                        principalColumn: "IdDiretorio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiretorioNotaModel_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_AnexosNota",
                columns: table => new
                {
                    IdAnexoNota = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TipoArquivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Dados = table.Column<byte[]>(type: "bytea", nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AnexosNota", x => x.IdAnexoNota);
                    table.ForeignKey(
                        name: "FK_PM_AnexosNota_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_ItensNota",
                columns: table => new
                {
                    IdItemNota = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Componente = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Sintoma = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Causa = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_ItensNota", x => x.IdItemNota);
                    table.ForeignKey(
                        name: "FK_PM_ItensNota_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Ordens",
                columns: table => new
                {
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroOrdem = table.Column<long>(type: "bigint", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusOrdem = table.Column<int>(type: "integer", nullable: true),
                    Criticidade = table.Column<int>(type: "integer", nullable: true),
                    Classificacao = table.Column<int>(type: "integer", nullable: false),
                    MetodoClassificacao = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    ClassificadoPor = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoOrdem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConfiabilidadeRestaurada = table.Column<int>(type: "integer", nullable: true),
                    InicioPlanejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimPlanejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InicioReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DescricaoBreve = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DescricaoLonga = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CriadaPorUsuario = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ModificadaPorUsuario = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DataModificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Escopo = table.Column<long>(type: "bigint", nullable: true),
                    IdAtivo = table.Column<int>(type: "integer", nullable: true),
                    IdLi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DnsIot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: true),
                    IdGpm = table.Column<long>(type: "bigint", nullable: true),
                    IdCtm = table.Column<long>(type: "bigint", nullable: true),
                    IdPlano = table.Column<long>(type: "bigint", nullable: true),
                    UsuarioModelIdUsuario = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Ordens", x => x.IdOrdem);
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Ctms_IdCtm",
                        column: x => x.IdCtm,
                        principalTable: "PM_Ctms",
                        principalColumn: "IdCTM");
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Gpms_IdGpm",
                        column: x => x.IdGpm,
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM");
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Planos_IdPlano",
                        column: x => x.IdPlano,
                        principalTable: "PM_Planos",
                        principalColumn: "IdPlano");
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Usuarios_UsuarioModelIdUsuario",
                        column: x => x.UsuarioModelIdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "PM_VariaveisNota",
                columns: table => new
                {
                    IdVariavel = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeVariavel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValorNumerico = table.Column<double>(type: "double precision", nullable: true),
                    UnidadeMedida = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Comentarios = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_VariaveisNota", x => x.IdVariavel);
                    table.ForeignKey(
                        name: "FK_PM_VariaveisNota_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                });

            migrationBuilder.CreateTable(
                name: "DadosRenovacao",
                columns: table => new
                {
                    IdDados = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UltimaRenovacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OrigemUR = table.Column<int>(type: "integer", nullable: false),
                    TipoDados = table.Column<int>(type: "integer", nullable: false),
                    Confiabilidade = table.Column<double>(type: "double precision", nullable: true),
                    IdAtivo = table.Column<int>(type: "integer", nullable: true),
                    IdLi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdNota = table.Column<long>(type: "bigint", nullable: true),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: true),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosRenovacao", x => x.IdDados);
                    table.ForeignKey(
                        name: "FK_DadosRenovacao_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                    table.ForeignKey(
                        name: "FK_DadosRenovacao_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem");
                    table.ForeignKey(
                        name: "FK_DadosRenovacao_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "DiretorioOrdemModel",
                columns: table => new
                {
                    IdDiretorio = table.Column<long>(type: "bigint", nullable: false),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false),
                    DiretorioIdDiretorio = table.Column<long>(type: "bigint", nullable: false),
                    OrdemIdOrdem = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiretorioOrdemModel", x => new { x.IdDiretorio, x.IdOrdem });
                    table.ForeignKey(
                        name: "FK_DiretorioOrdemModel_PM_Diretorios_DiretorioIdDiretorio",
                        column: x => x.DiretorioIdDiretorio,
                        principalTable: "PM_Diretorios",
                        principalColumn: "IdDiretorio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiretorioOrdemModel_PM_Ordens_OrdemIdOrdem",
                        column: x => x.OrdemIdOrdem,
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_AnexosOrdem",
                columns: table => new
                {
                    IdAnexoOrdem = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TipoArquivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoAnexo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Dados = table.Column<byte[]>(type: "bytea", nullable: true),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AnexosOrdem", x => x.IdAnexoOrdem);
                    table.ForeignKey(
                        name: "FK_PM_AnexosOrdem_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Operacoes",
                columns: table => new
                {
                    IdOperacao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Duracao = table.Column<double>(type: "double precision", nullable: true),
                    NdeProfissionais = table.Column<int>(type: "integer", nullable: true),
                    InicioEsperado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimEsperado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IdCTM = table.Column<long>(type: "bigint", nullable: true),
                    IdTipoManutencao = table.Column<long>(type: "bigint", nullable: true),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Operacoes", x => x.IdOperacao);
                    table.ForeignKey(
                        name: "FK_PM_Operacoes_PM_Ctms_IdCTM",
                        column: x => x.IdCTM,
                        principalTable: "PM_Ctms",
                        principalColumn: "IdCTM");
                    table.ForeignKey(
                        name: "FK_PM_Operacoes_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem");
                    table.ForeignKey(
                        name: "FK_PM_Operacoes_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposPlano",
                columns: table => new
                {
                    IdTipoPlano = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Visivel = table.Column<bool>(type: "boolean", nullable: true),
                    Dias = table.Column<int>(type: "integer", nullable: true),
                    ToleranciaPositiva = table.Column<double>(type: "double precision", nullable: true),
                    ToleranciaNegativa = table.Column<double>(type: "double precision", nullable: true),
                    Continua = table.Column<bool>(type: "boolean", nullable: true),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    IdTipoEstrategia = table.Column<long>(type: "bigint", nullable: true),
                    IdPlano = table.Column<long>(type: "bigint", nullable: true),
                    IdTipoManutencao = table.Column<long>(type: "bigint", nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: true),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: true),
                    IdTipoCep = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposPlano", x => x.IdTipoPlano);
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_Planos_IdPlano",
                        column: x => x.IdPlano,
                        principalTable: "PM_Planos",
                        principalColumn: "IdPlano");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposCep_IdTipoCep",
                        column: x => x.IdTipoCep,
                        principalTable: "PM_TiposCep",
                        principalColumn: "IdTipoCep");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposEstrategia_IdTipoEstrategia",
                        column: x => x.IdTipoEstrategia,
                        principalTable: "PM_TiposEstrategia",
                        principalColumn: "IdTipoEstrategia");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_FerramentasOperacoes",
                columns: table => new
                {
                    IdFerramenta = table.Column<long>(type: "bigint", nullable: false),
                    IdOperacao = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_FerramentasOperacoes", x => new { x.IdFerramenta, x.IdOperacao });
                    table.ForeignKey(
                        name: "FK_PM_FerramentasOperacoes_PM_Ferramentas_IdFerramenta",
                        column: x => x.IdFerramenta,
                        principalTable: "PM_Ferramentas",
                        principalColumn: "IdFerramenta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_FerramentasOperacoes_PM_Operacoes_IdOperacao",
                        column: x => x.IdOperacao,
                        principalTable: "PM_Operacoes",
                        principalColumn: "IdOperacao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PM_ConfiguracoesGlobais",
                columns: new[] { "Id", "ClassificacaoAutomatica", "CriarFichasPreventivas", "CriterioConfiabilidadeRenovacao", "CriterioDataRenovacao", "DataModificacao", "FichasConfiabilidade", "ModificadoPorUsuario" },
                values: new object[] { 1, 0, 0, 2, 3, new DateTime(2026, 8, 24, 14, 13, 48, 93, DateTimeKind.Utc).AddTicks(1130), 2, null });

            migrationBuilder.CreateIndex(
                name: "IX_DadosRenovacao_IdNota",
                table: "DadosRenovacao",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_DadosRenovacao_IdOrdem",
                table: "DadosRenovacao",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_DadosRenovacao_IdUsuario",
                table: "DadosRenovacao",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_DiretorioNotaModel_IdNota",
                table: "DiretorioNotaModel",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_DiretorioOrdemModel_DiretorioIdDiretorio",
                table: "DiretorioOrdemModel",
                column: "DiretorioIdDiretorio");

            migrationBuilder.CreateIndex(
                name: "IX_DiretorioOrdemModel_OrdemIdOrdem",
                table: "DiretorioOrdemModel",
                column: "OrdemIdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AlmoxarifadoUsuario_IdUsuario",
                table: "PM_AlmoxarifadoUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AnexosNota_IdNota",
                table: "PM_AnexosNota",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AnexosOrdem_IdOrdem",
                table: "PM_AnexosOrdem",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ctms_IdCPM",
                table: "PM_Ctms",
                column: "IdCPM");

            migrationBuilder.CreateIndex(
                name: "IX_PM_CtmUsuario_IdUsuario",
                table: "PM_CtmUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Diretorios_IdDiretorioPai",
                table: "PM_Diretorios",
                column: "IdDiretorioPai");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ferramentas_IdAlmoxarifado",
                table: "PM_Ferramentas",
                column: "IdAlmoxarifado");

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasOperacoes_IdOperacao",
                table: "PM_FerramentasOperacoes",
                column: "IdOperacao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_GpmLi_IdUsuario",
                table: "PM_GpmLi",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Gpms_IdCPM",
                table: "PM_Gpms",
                column: "IdCPM");

            migrationBuilder.CreateIndex(
                name: "IX_PM_GpmUsuario_IdUsuario",
                table: "PM_GpmUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_ItensNota_IdNota",
                table: "PM_ItensNota",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Notas_IdGpm",
                table: "PM_Notas",
                column: "IdGpm");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Notas_IdPlano",
                table: "PM_Notas",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Notas_IdUsuario",
                table: "PM_Notas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Operacoes_IdCTM",
                table: "PM_Operacoes",
                column: "IdCTM");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Operacoes_IdOrdem",
                table: "PM_Operacoes",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Operacoes_IdTipoManutencao",
                table: "PM_Operacoes",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_IdCtm",
                table: "PM_Ordens",
                column: "IdCtm");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_IdGpm",
                table: "PM_Ordens",
                column: "IdGpm");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_IdNota",
                table: "PM_Ordens",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_IdPlano",
                table: "PM_Ordens",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_UsuarioModelIdUsuario",
                table: "PM_Ordens",
                column: "UsuarioModelIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Planos_IdEstrategia",
                table: "PM_Planos",
                column: "IdEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdEstrategia",
                table: "PM_TiposEstrategia",
                column: "IdEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdTipoCep",
                table: "PM_TiposEstrategia",
                column: "IdTipoCep");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdTipoManutencao",
                table: "PM_TiposEstrategia",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdNota",
                table: "PM_TiposPlano",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdOrdem",
                table: "PM_TiposPlano",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdPlano",
                table: "PM_TiposPlano",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdTipoCep",
                table: "PM_TiposPlano",
                column: "IdTipoCep");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdTipoEstrategia",
                table: "PM_TiposPlano",
                column: "IdTipoEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdTipoManutencao",
                table: "PM_TiposPlano",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_VariaveisNota_IdNota",
                table: "PM_VariaveisNota",
                column: "IdNota");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosRenovacao");

            migrationBuilder.DropTable(
                name: "DiretorioNotaModel");

            migrationBuilder.DropTable(
                name: "DiretorioOrdemModel");

            migrationBuilder.DropTable(
                name: "PM_AlmoxarifadoUsuario");

            migrationBuilder.DropTable(
                name: "PM_AnexosNota");

            migrationBuilder.DropTable(
                name: "PM_AnexosOrdem");

            migrationBuilder.DropTable(
                name: "PM_Auditorias");

            migrationBuilder.DropTable(
                name: "PM_ConfiguracoesGlobais");

            migrationBuilder.DropTable(
                name: "PM_CtmUsuario");

            migrationBuilder.DropTable(
                name: "PM_FerramentasOperacoes");

            migrationBuilder.DropTable(
                name: "PM_GpmLi");

            migrationBuilder.DropTable(
                name: "PM_GpmUsuario");

            migrationBuilder.DropTable(
                name: "PM_Imobilizados");

            migrationBuilder.DropTable(
                name: "PM_ItensNota");

            migrationBuilder.DropTable(
                name: "PM_Reliability");

            migrationBuilder.DropTable(
                name: "PM_RuleClassification");

            migrationBuilder.DropTable(
                name: "PM_TiposPlano");

            migrationBuilder.DropTable(
                name: "PM_VariaveisNota");

            migrationBuilder.DropTable(
                name: "RegraClassificacaoDto");

            migrationBuilder.DropTable(
                name: "PM_Diretorios");

            migrationBuilder.DropTable(
                name: "PM_Ferramentas");

            migrationBuilder.DropTable(
                name: "PM_Operacoes");

            migrationBuilder.DropTable(
                name: "PM_TiposEstrategia");

            migrationBuilder.DropTable(
                name: "PM_Almoxarifados");

            migrationBuilder.DropTable(
                name: "PM_Ordens");

            migrationBuilder.DropTable(
                name: "PM_TiposCep");

            migrationBuilder.DropTable(
                name: "PM_TiposManutencao");

            migrationBuilder.DropTable(
                name: "PM_Ctms");

            migrationBuilder.DropTable(
                name: "PM_Notas");

            migrationBuilder.DropTable(
                name: "PM_Gpms");

            migrationBuilder.DropTable(
                name: "PM_Planos");

            migrationBuilder.DropTable(
                name: "PM_Usuarios");

            migrationBuilder.DropTable(
                name: "PM_Cpms");

            migrationBuilder.DropTable(
                name: "Estrategias");
        }
    }
}
