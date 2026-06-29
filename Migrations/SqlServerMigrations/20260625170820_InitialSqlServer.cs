using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_pm_back.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Estrategias",
                schema: "dbo",
                columns: table => new
                {
                    IdEstrategia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ciclos = table.Column<int>(type: "int", nullable: true),
                    Habilitada = table.Column<bool>(type: "bit", nullable: false),
                    TipoAtivoAplicavel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroCiclos = table.Column<int>(type: "int", nullable: true),
                    CicloContinuo = table.Column<bool>(type: "bit", nullable: true),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estrategias", x => x.IdEstrategia);
                });

            migrationBuilder.CreateTable(
                name: "PM_Almoxarifados",
                schema: "dbo",
                columns: table => new
                {
                    IdAlmoxarifado = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Organizacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Almoxarifados", x => x.IdAlmoxarifado);
                });

            migrationBuilder.CreateTable(
                name: "PM_ClassificationRules",
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
                    table.PrimaryKey("PK_PM_ClassificationRules", x => x.IdRegra);
                });

            migrationBuilder.CreateTable(
                name: "PM_Cpms",
                schema: "dbo",
                columns: table => new
                {
                    IdCPM = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCentro = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Organizacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Cpms", x => x.IdCPM);
                });

            migrationBuilder.CreateTable(
                name: "PM_Imobilizados",
                schema: "dbo",
                columns: table => new
                {
                    IdImobilizado = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Denominacao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nserie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataIncorporacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Moeda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Depreciacao = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorAtual = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InfoAdicionais = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdAtivo = table.Column<int>(type: "int", nullable: true),
                    IdLi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Imobilizados", x => x.IdImobilizado);
                });

            migrationBuilder.CreateTable(
                name: "PM_Reliability",
                schema: "dbo",
                columns: table => new
                {
                    IdLi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdsNotas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdsOrdens = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Reliability", x => x.IdLi);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposCep",
                schema: "dbo",
                columns: table => new
                {
                    IdTipoCep = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoAtivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Confiabilidade = table.Column<double>(type: "float", nullable: true),
                    Habilitada = table.Column<bool>(type: "bit", nullable: true),
                    CampoIndefinido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcoesFalha = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposCep", x => x.IdTipoCep);
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposManutencao",
                schema: "dbo",
                columns: table => new
                {
                    IdTipoManutencao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<int>(type: "int", maxLength: 100, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoAtivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Confiabilidade = table.Column<double>(type: "float", nullable: true),
                    Habilitada = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_TiposManutencao", x => x.IdTipoManutencao);
                });

            migrationBuilder.CreateTable(
                name: "PM_Usuarios",
                schema: "dbo",
                columns: table => new
                {
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Permissoes = table.Column<int>(type: "int", nullable: true),
                    NivelPermissao = table.Column<int>(type: "int", nullable: true),
                    Escopos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "PM_Planos",
                schema: "dbo",
                columns: table => new
                {
                    IdPlano = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Prioridade = table.Column<int>(type: "int", nullable: true),
                    DataInicios = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GerarAutomaticamente = table.Column<int>(type: "int", nullable: true),
                    Antecedencia = table.Column<int>(type: "int", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdAtivo = table.Column<int>(type: "int", nullable: true),
                    IdLi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdEstrategia = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Planos", x => x.IdPlano);
                    table.ForeignKey(
                        name: "FK_PM_Planos_Estrategias_IdEstrategia",
                        column: x => x.IdEstrategia,
                        principalSchema: "dbo",
                        principalTable: "Estrategias",
                        principalColumn: "IdEstrategia");
                });

            migrationBuilder.CreateTable(
                name: "PM_Ferramentas",
                schema: "dbo",
                columns: table => new
                {
                    IdFerramenta = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    TipoFerramenta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PesoBruto = table.Column<double>(type: "float", nullable: false),
                    PesoLiquido = table.Column<double>(type: "float", nullable: false),
                    UmPesoBruto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UmPesoLiquido = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Volume = table.Column<double>(type: "float", nullable: false),
                    UmVolume = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Dimensao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UmDimensao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Fabricante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ValorCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moeda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IdLI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdEscopo = table.Column<long>(type: "bigint", nullable: false),
                    IdAlmoxarifado = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Ferramentas", x => x.IdFerramenta);
                    table.ForeignKey(
                        name: "FK_PM_Ferramentas_PM_Almoxarifados_IdAlmoxarifado",
                        column: x => x.IdAlmoxarifado,
                        principalSchema: "dbo",
                        principalTable: "PM_Almoxarifados",
                        principalColumn: "IdAlmoxarifado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Gpms",
                schema: "dbo",
                columns: table => new
                {
                    IdGPM = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IdCPM = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Gpms", x => x.IdGPM);
                    table.ForeignKey(
                        name: "FK_PM_Gpms_PM_Cpms_IdCPM",
                        column: x => x.IdCPM,
                        principalSchema: "dbo",
                        principalTable: "PM_Cpms",
                        principalColumn: "IdCPM");
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposEstrategia",
                schema: "dbo",
                columns: table => new
                {
                    IdTipoEstrategia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dias = table.Column<int>(type: "int", nullable: true),
                    ToleranciaPositiva = table.Column<double>(type: "float", nullable: true),
                    ToleranciaNegativa = table.Column<double>(type: "float", nullable: true),
                    Continua = table.Column<bool>(type: "bit", nullable: true),
                    Categoria = table.Column<int>(type: "int", nullable: false),
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
                        principalSchema: "dbo",
                        principalTable: "Estrategias",
                        principalColumn: "IdEstrategia");
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_TiposCep_IdTipoCep",
                        column: x => x.IdTipoCep,
                        principalSchema: "dbo",
                        principalTable: "PM_TiposCep",
                        principalColumn: "IdTipoCep");
                    table.ForeignKey(
                        name: "FK_PM_TiposEstrategia_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalSchema: "dbo",
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_AlmoxarifadoUsuario",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "PM_Almoxarifados",
                        principalColumn: "IdAlmoxarifado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_AlmoxarifadoUsuario_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "dbo",
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Notas",
                schema: "dbo",
                columns: table => new
                {
                    IdNota = table.Column<long>(type: "bigint", nullable: false),
                    SapCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroNota = table.Column<long>(type: "bigint", nullable: false),
                    DataNota = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoNota = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenciaLocalizacaoAtivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Finalidade = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Prioridade = table.Column<int>(type: "int", nullable: true),
                    FimDesejado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAvaria = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAvaria = table.Column<TimeSpan>(type: "time", nullable: true),
                    StatusNota = table.Column<int>(type: "int", nullable: true),
                    EquipamentoDisponivel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConfiabilidadeRestaurada = table.Column<int>(type: "int", nullable: true),
                    DescricaoBreve = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnomaliaObservada = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ObservacoesComplementares = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Classificacao = table.Column<int>(type: "int", nullable: false),
                    MetodoClassificacao = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ClassificadoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadaPorUsuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ModificadaPorUsuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DataModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Escopo = table.Column<long>(type: "bigint", nullable: true),
                    IdPlano = table.Column<long>(type: "bigint", nullable: true),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: true),
                    IdAtivo = table.Column<int>(type: "int", nullable: true),
                    IdLi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DnsIot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Notas", x => x.IdNota);
                    table.ForeignKey(
                        name: "FK_PM_Notas_PM_Planos_IdPlano",
                        column: x => x.IdPlano,
                        principalSchema: "dbo",
                        principalTable: "PM_Planos",
                        principalColumn: "IdPlano");
                    table.ForeignKey(
                        name: "FK_PM_Notas_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "dbo",
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "PM_Ctms",
                schema: "dbo",
                columns: table => new
                {
                    IdCTM = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AreaCTM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IdCPM = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Ctms", x => x.IdCTM);
                    table.ForeignKey(
                        name: "FK_PM_Ctms_PM_Cpms_IdCPM",
                        column: x => x.IdCPM,
                        principalSchema: "dbo",
                        principalTable: "PM_Cpms",
                        principalColumn: "IdCPM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_Ctms_PM_Gpms_IdCTM",
                        column: x => x.IdCTM,
                        principalSchema: "dbo",
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_GpmLi",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_GpmLi_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "dbo",
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_GpmUsuario",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "PM_Gpms",
                        principalColumn: "IdGPM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_GpmUsuario_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "dbo",
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_AnexosNota",
                schema: "dbo",
                columns: table => new
                {
                    IdAnexoNota = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dados = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AnexosNota", x => x.IdAnexoNota);
                    table.ForeignKey(
                        name: "FK_PM_AnexosNota_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalSchema: "dbo",
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_ItensNota",
                schema: "dbo",
                columns: table => new
                {
                    IdItemNota = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Componente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Sintoma = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Causa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_ItensNota", x => x.IdItemNota);
                    table.ForeignKey(
                        name: "FK_PM_ItensNota_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalSchema: "dbo",
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Ordens",
                schema: "dbo",
                columns: table => new
                {
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false),
                    NumeroOrdem = table.Column<long>(type: "bigint", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusOrdem = table.Column<int>(type: "int", nullable: true),
                    Criticidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Classificacao = table.Column<int>(type: "int", nullable: false),
                    MetodoClassificacao = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ClassificadoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoOrdem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConfiabilidadeRestaurada = table.Column<int>(type: "int", nullable: true),
                    InicioPlanejado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FimPlanejado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InicioReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FimReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DescricaoBreve = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadaPorUsuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ModificadaPorUsuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DataModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Escopo = table.Column<long>(type: "bigint", nullable: true),
                    IdAtivo = table.Column<int>(type: "int", nullable: true),
                    IdLi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DnsIot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: true),
                    IdPlano = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_Ordens", x => x.IdOrdem);
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalSchema: "dbo",
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                    table.ForeignKey(
                        name: "FK_PM_Ordens_PM_Planos_IdPlano",
                        column: x => x.IdPlano,
                        principalSchema: "dbo",
                        principalTable: "PM_Planos",
                        principalColumn: "IdPlano");
                });

            migrationBuilder.CreateTable(
                name: "PM_VariaveisNota",
                schema: "dbo",
                columns: table => new
                {
                    IdVariavel = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeVariavel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValorNumerico = table.Column<double>(type: "float", nullable: true),
                    UnidadeMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Comentarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdNota = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_VariaveisNota", x => x.IdVariavel);
                    table.ForeignKey(
                        name: "FK_PM_VariaveisNota_PM_Notas_IdNota",
                        column: x => x.IdNota,
                        principalSchema: "dbo",
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                });

            migrationBuilder.CreateTable(
                name: "PM_CtmUsuario",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "PM_Ctms",
                        principalColumn: "IdCTM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_CtmUsuario_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "dbo",
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadosRenovacao",
                schema: "dbo",
                columns: table => new
                {
                    IdDados = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UltimaRenovacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrigemUR = table.Column<int>(type: "int", nullable: false),
                    TipoDados = table.Column<int>(type: "int", nullable: false),
                    Confiabilidade = table.Column<double>(type: "float", nullable: true),
                    IdAtivo = table.Column<int>(type: "int", nullable: true),
                    IdLi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                        principalSchema: "dbo",
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                    table.ForeignKey(
                        name: "FK_DadosRenovacao_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalSchema: "dbo",
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem");
                    table.ForeignKey(
                        name: "FK_DadosRenovacao_PM_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "dbo",
                        principalTable: "PM_Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "PM_AnexosOrdem",
                schema: "dbo",
                columns: table => new
                {
                    IdAnexoOrdem = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoAnexo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Dados = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PM_AnexosOrdem", x => x.IdAnexoOrdem);
                    table.ForeignKey(
                        name: "FK_PM_AnexosOrdem_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalSchema: "dbo",
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PM_Operacoes",
                schema: "dbo",
                columns: table => new
                {
                    IdOperacao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Duracao = table.Column<double>(type: "float", nullable: true),
                    NdeProfissionais = table.Column<int>(type: "int", nullable: true),
                    InicioEsperado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FimEsperado = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                        principalSchema: "dbo",
                        principalTable: "PM_Ctms",
                        principalColumn: "IdCTM");
                    table.ForeignKey(
                        name: "FK_PM_Operacoes_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalSchema: "dbo",
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem");
                    table.ForeignKey(
                        name: "FK_PM_Operacoes_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalSchema: "dbo",
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_TiposPlano",
                schema: "dbo",
                columns: table => new
                {
                    IdTipoPlano = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Visivel = table.Column<bool>(type: "bit", nullable: true),
                    Dias = table.Column<int>(type: "int", nullable: true),
                    ToleranciaPositiva = table.Column<double>(type: "float", nullable: true),
                    ToleranciaNegativa = table.Column<double>(type: "float", nullable: true),
                    Continua = table.Column<bool>(type: "bit", nullable: true),
                    Categoria = table.Column<int>(type: "int", nullable: false),
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
                        principalSchema: "dbo",
                        principalTable: "PM_Notas",
                        principalColumn: "IdNota");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_Ordens_IdOrdem",
                        column: x => x.IdOrdem,
                        principalSchema: "dbo",
                        principalTable: "PM_Ordens",
                        principalColumn: "IdOrdem");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_Planos_IdPlano",
                        column: x => x.IdPlano,
                        principalSchema: "dbo",
                        principalTable: "PM_Planos",
                        principalColumn: "IdPlano");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposCep_IdTipoCep",
                        column: x => x.IdTipoCep,
                        principalSchema: "dbo",
                        principalTable: "PM_TiposCep",
                        principalColumn: "IdTipoCep");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposEstrategia_IdTipoEstrategia",
                        column: x => x.IdTipoEstrategia,
                        principalSchema: "dbo",
                        principalTable: "PM_TiposEstrategia",
                        principalColumn: "IdTipoEstrategia");
                    table.ForeignKey(
                        name: "FK_PM_TiposPlano_PM_TiposManutencao_IdTipoManutencao",
                        column: x => x.IdTipoManutencao,
                        principalSchema: "dbo",
                        principalTable: "PM_TiposManutencao",
                        principalColumn: "IdTipoManutencao");
                });

            migrationBuilder.CreateTable(
                name: "PM_FerramentasOperacoes",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "PM_Ferramentas",
                        principalColumn: "IdFerramenta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PM_FerramentasOperacoes_PM_Operacoes_IdOperacao",
                        column: x => x.IdOperacao,
                        principalSchema: "dbo",
                        principalTable: "PM_Operacoes",
                        principalColumn: "IdOperacao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DadosRenovacao_IdNota",
                schema: "dbo",
                table: "DadosRenovacao",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_DadosRenovacao_IdOrdem",
                schema: "dbo",
                table: "DadosRenovacao",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_DadosRenovacao_IdUsuario",
                schema: "dbo",
                table: "DadosRenovacao",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AlmoxarifadoUsuario_IdUsuario",
                schema: "dbo",
                table: "PM_AlmoxarifadoUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AnexosNota_IdNota",
                schema: "dbo",
                table: "PM_AnexosNota",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_AnexosOrdem_IdOrdem",
                schema: "dbo",
                table: "PM_AnexosOrdem",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ctms_IdCPM",
                schema: "dbo",
                table: "PM_Ctms",
                column: "IdCPM");

            migrationBuilder.CreateIndex(
                name: "IX_PM_CtmUsuario_IdUsuario",
                schema: "dbo",
                table: "PM_CtmUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ferramentas_IdAlmoxarifado",
                schema: "dbo",
                table: "PM_Ferramentas",
                column: "IdAlmoxarifado");

            migrationBuilder.CreateIndex(
                name: "IX_PM_FerramentasOperacoes_IdOperacao",
                schema: "dbo",
                table: "PM_FerramentasOperacoes",
                column: "IdOperacao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_GpmLi_IdUsuario",
                schema: "dbo",
                table: "PM_GpmLi",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Gpms_IdCPM",
                schema: "dbo",
                table: "PM_Gpms",
                column: "IdCPM");

            migrationBuilder.CreateIndex(
                name: "IX_PM_GpmUsuario_IdUsuario",
                schema: "dbo",
                table: "PM_GpmUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_ItensNota_IdNota",
                schema: "dbo",
                table: "PM_ItensNota",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Notas_IdPlano",
                schema: "dbo",
                table: "PM_Notas",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Notas_IdUsuario",
                schema: "dbo",
                table: "PM_Notas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Operacoes_IdCTM",
                schema: "dbo",
                table: "PM_Operacoes",
                column: "IdCTM");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Operacoes_IdOrdem",
                schema: "dbo",
                table: "PM_Operacoes",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Operacoes_IdTipoManutencao",
                schema: "dbo",
                table: "PM_Operacoes",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_IdNota",
                schema: "dbo",
                table: "PM_Ordens",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Ordens_IdPlano",
                schema: "dbo",
                table: "PM_Ordens",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_PM_Planos_IdEstrategia",
                schema: "dbo",
                table: "PM_Planos",
                column: "IdEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdEstrategia",
                schema: "dbo",
                table: "PM_TiposEstrategia",
                column: "IdEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdTipoCep",
                schema: "dbo",
                table: "PM_TiposEstrategia",
                column: "IdTipoCep");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposEstrategia_IdTipoManutencao",
                schema: "dbo",
                table: "PM_TiposEstrategia",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdNota",
                schema: "dbo",
                table: "PM_TiposPlano",
                column: "IdNota");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdOrdem",
                schema: "dbo",
                table: "PM_TiposPlano",
                column: "IdOrdem");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdPlano",
                schema: "dbo",
                table: "PM_TiposPlano",
                column: "IdPlano");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdTipoCep",
                schema: "dbo",
                table: "PM_TiposPlano",
                column: "IdTipoCep");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdTipoEstrategia",
                schema: "dbo",
                table: "PM_TiposPlano",
                column: "IdTipoEstrategia");

            migrationBuilder.CreateIndex(
                name: "IX_PM_TiposPlano_IdTipoManutencao",
                schema: "dbo",
                table: "PM_TiposPlano",
                column: "IdTipoManutencao");

            migrationBuilder.CreateIndex(
                name: "IX_PM_VariaveisNota_IdNota",
                schema: "dbo",
                table: "PM_VariaveisNota",
                column: "IdNota");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosRenovacao",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_AlmoxarifadoUsuario",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_AnexosNota",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_AnexosOrdem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_ClassificationRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_CtmUsuario",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_FerramentasOperacoes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_GpmLi",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_GpmUsuario",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Imobilizados",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_ItensNota",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Reliability",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_TiposPlano",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_VariaveisNota",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Ferramentas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Operacoes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_TiposEstrategia",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Almoxarifados",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Ctms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Ordens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_TiposCep",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_TiposManutencao",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Gpms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Notas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Cpms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Planos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PM_Usuarios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Estrategias",
                schema: "dbo");
        }
    }
}
