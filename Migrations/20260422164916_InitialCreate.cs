using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app_pm_back.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcaoNotaCongas",
                columns: table => new
                {
                    IdAcao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaOperac = table.Column<int>(type: "integer", nullable: true),
                    Atividade = table.Column<string>(type: "text", nullable: true),
                    TpCatalogo = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    CanalDistrib = table.Column<string>(type: "text", nullable: true),
                    CentroCusto = table.Column<string>(type: "text", nullable: true),
                    CenLocaliz = table.Column<string>(type: "text", nullable: true),
                    CenPCenTrab = table.Column<string>(type: "text", nullable: true),
                    CentroPlanej = table.Column<string>(type: "text", nullable: true),
                    CenTrabRespon = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    ClasseDefeitos = table.Column<string>(type: "text", nullable: true),
                    Cliente = table.Column<string>(type: "text", nullable: true),
                    Codificacao = table.Column<string>(type: "text", nullable: true),
                    CodeParteObj = table.Column<string>(type: "text", nullable: true),
                    CodigoABC = table.Column<string>(type: "text", nullable: true),
                    CodigoDeAcao = table.Column<string>(type: "text", nullable: true),
                    CodigoDoDano = table.Column<string>(type: "text", nullable: true),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true),
                    ConclDesejada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CondInstAntAvar = table.Column<string>(type: "text", nullable: true),
                    CondInstDepAvar = table.Column<string>(type: "text", nullable: true),
                    CondInstAposMed = table.Column<string>(type: "text", nullable: true),
                    Conjunto1 = table.Column<string>(type: "text", nullable: true),
                    Conjunto2 = table.Column<string>(type: "text", nullable: true),
                    Consequencia = table.Column<string>(type: "text", nullable: true),
                    ControlTecnEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ContrTecnDe = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadoAs = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    DataDaNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Inicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtReferencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPedido = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Fim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DdsDispositivo = table.Column<string>(type: "text", nullable: true),
                    DenomConjunto1 = table.Column<string>(type: "text", nullable: true),
                    DenomConjunto2 = table.Column<string>(type: "text", nullable: true),
                    Denominacao1 = table.Column<string>(type: "text", nullable: true),
                    Denominacao2 = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    DispAntesAvaria = table.Column<string>(type: "text", nullable: true),
                    DispAposMed = table.Column<string>(type: "text", nullable: true),
                    DispDepoisAvar = table.Column<string>(type: "text", nullable: true),
                    Divisao = table.Column<string>(type: "text", nullable: true),
                    DocVendas = table.Column<string>(type: "text", nullable: true),
                    DuracParada = table.Column<string>(type: "text", nullable: true),
                    Empresa = table.Column<string>(type: "text", nullable: true),
                    DataEncermto = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Equipamento = table.Column<string>(type: "text", nullable: true),
                    EquipamAfetado = table.Column<string>(type: "text", nullable: true),
                    FatorQuantidad = table.Column<decimal>(type: "numeric", nullable: true),
                    FimAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GrpCodigos1 = table.Column<string>(type: "text", nullable: true),
                    GrpCodigos2 = table.Column<string>(type: "text", nullable: true),
                    GrpPlnjPM = table.Column<string>(type: "text", nullable: true),
                    GrupoDeCodes = table.Column<string>(type: "text", nullable: true),
                    Hora1 = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Hora2 = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraDaNota = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraEncermto = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraInicDes = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ModificadoAs = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraReferencia = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraFimAvaria = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HFinalDesej = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraInicioAvar = table.Column<TimeSpan>(type: "interval", nullable: true),
                    IdiomPrincipal = table.Column<string>(type: "text", nullable: true),
                    Imobilizado = table.Column<string>(type: "text", nullable: true),
                    InicioAvar = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InicioDesejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Item1 = table.Column<string>(type: "text", nullable: true),
                    Item2 = table.Column<string>(type: "text", nullable: true),
                    Local = table.Column<string>(type: "text", nullable: true),
                    LocInstalacao = table.Column<string>(type: "text", nullable: true),
                    LocInstAfetad = table.Column<string>(type: "text", nullable: true),
                    Localizacao = table.Column<string>(type: "text", nullable: true),
                    LocalClasContb = table.Column<string>(type: "text", nullable: true),
                    Material = table.Column<string>(type: "text", nullable: true),
                    ModificadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificadoPor = table.Column<string>(type: "text", nullable: true),
                    Moeda = table.Column<string>(type: "text", nullable: true),
                    NroDeOrdenacao1 = table.Column<string>(type: "text", nullable: true),
                    NroDeOrdenacao2 = table.Column<string>(type: "text", nullable: true),
                    NroDeSerie = table.Column<string>(type: "text", nullable: true),
                    NroDoPedido = table.Column<string>(type: "text", nullable: true),
                    NroEndereco = table.Column<string>(type: "text", nullable: true),
                    NomeLista = table.Column<string>(type: "text", nullable: true),
                    NroObjeto = table.Column<string>(type: "text", nullable: true),
                    Nota = table.Column<string>(type: "text", nullable: true),
                    Notificador = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<string>(type: "text", nullable: true),
                    OrdemCliente = table.Column<string>(type: "text", nullable: true),
                    OrgVendas = table.Column<string>(type: "text", nullable: true),
                    Parada = table.Column<string>(type: "text", nullable: true),
                    GrpCodigos3 = table.Column<string>(type: "text", nullable: true),
                    PerfCatalogo = table.Column<string>(type: "text", nullable: true),
                    Prioridade1 = table.Column<string>(type: "text", nullable: true),
                    Regiao = table.Column<string>(type: "text", nullable: true),
                    Rua = table.Column<string>(type: "text", nullable: true),
                    SetorAtividade = table.Column<string>(type: "text", nullable: true),
                    StatusSistema = table.Column<string>(type: "text", nullable: true),
                    StatusUsuario = table.Column<string>(type: "text", nullable: true),
                    SubNro = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Denominacao3 = table.Column<string>(type: "text", nullable: true),
                    TextoCodeAcao = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCodif = table.Column<string>(type: "text", nullable: true),
                    TxtCodPartOb = table.Column<string>(type: "text", nullable: true),
                    TxtAtiv = table.Column<string>(type: "text", nullable: true),
                    TxtCodeProb = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodAc = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodPr = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodPartOb = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodific = table.Column<string>(type: "text", nullable: true),
                    TxtDescritivo = table.Column<string>(type: "text", nullable: true),
                    Texto = table.Column<string>(type: "text", nullable: true),
                    Prioridade2 = table.Column<string>(type: "text", nullable: true),
                    TipoDeNota = table.Column<string>(type: "text", nullable: true),
                    TipoPrioridade = table.Column<string>(type: "text", nullable: true),
                    Unidade = table.Column<string>(type: "text", nullable: true),
                    Versao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcaoNotaCongas", x => x.IdAcao);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    IdAnexo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeAnexo = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: false),
                    Dados = table.Column<byte[]>(type: "bytea", nullable: false),
                    ReferenciaId = table.Column<long>(type: "bigint", nullable: false),
                    TipoReferencia = table.Column<string>(type: "text", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.IdAnexo);
                });

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
                name: "EstrategiasManutencao",
                columns: table => new
                {
                    IdEstrategia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Denominacao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    Ciclos = table.Column<int>(type: "integer", nullable: false),
                    NumeroCiclos = table.Column<int>(type: "integer", nullable: false),
                    TipoAtivoAplicavel = table.Column<string>(type: "text", nullable: false)
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
                name: "NotasPetro",
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
                    table.PrimaryKey("PK_NotasPetro", x => x.IdNota);
                });

            migrationBuilder.CreateTable(
                name: "Regras",
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
                    table.PrimaryKey("PK_Regras", x => x.IdRegra);
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
                name: "PlanosManutencaoNormalizados",
                columns: table => new
                {
                    CodigoPlano = table.Column<long>(type: "bigint", maxLength: 40, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomePlano = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    GerarAutomaticamente = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Antencedencia = table.Column<int>(type: "integer", nullable: false),
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
                name: "TipoEstrategiaModel",
                columns: table => new
                {
                    IdTipoEstrategia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeTipo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Meses = table.Column<int>(type: "integer", nullable: false),
                    ToleranciaPositiva = table.Column<int>(type: "integer", nullable: false),
                    UnidadeToleranciaPositiva = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ToleranciaNegativa = table.Column<int>(type: "integer", nullable: false),
                    UnidadeToleranciaNegativa = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    EstrategiaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEstrategiaModel", x => x.IdTipoEstrategia);
                    table.ForeignKey(
                        name: "FK_TipoEstrategiaModel_EstrategiasManutencao_EstrategiaId",
                        column: x => x.EstrategiaId,
                        principalTable: "EstrategiasManutencao",
                        principalColumn: "IdEstrategia");
                });

            migrationBuilder.CreateTable(
                name: "OrdensPetro",
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
                    table.PrimaryKey("PK_OrdensPetro", x => x.IdOrdem);
                    table.ForeignKey(
                        name: "FK_OrdensPetro_NotasPetro_IdNota",
                        column: x => x.IdNota,
                        principalTable: "NotasPetro",
                        principalColumn: "IdNota",
                        onDelete: ReferentialAction.Cascade);
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
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", maxLength: 40, nullable: false)
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
                name: "TipoManutencaoModel",
                columns: table => new
                {
                    IdTipoManutencao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoTipo = table.Column<int>(type: "integer", nullable: false),
                    NomeTipoManutencao = table.Column<string>(type: "text", nullable: false),
                    Habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    Confiabilidade = table.Column<int>(type: "integer", nullable: false),
                    TipoAtivo = table.Column<string>(type: "text", nullable: false),
                    TipoEstrategiaId = table.Column<int>(type: "integer", nullable: true),
                    PlanoManutencaoId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoManutencaoModel", x => x.IdTipoManutencao);
                    table.ForeignKey(
                        name: "FK_TipoManutencaoModel_PlanosManutencaoNormalizados_PlanoManut~",
                        column: x => x.PlanoManutencaoId,
                        principalTable: "PlanosManutencaoNormalizados",
                        principalColumn: "CodigoPlano");
                    table.ForeignKey(
                        name: "FK_TipoManutencaoModel_TipoEstrategiaModel_TipoEstrategiaId",
                        column: x => x.TipoEstrategiaId,
                        principalTable: "TipoEstrategiaModel",
                        principalColumn: "IdTipoEstrategia");
                });

            migrationBuilder.CreateTable(
                name: "EquipamentosAtivos",
                columns: table => new
                {
                    TagEquipamento = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    LocalInstalacaoCodigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DetalhesTecnicos = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false)
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
                name: "FerramentasTipoModel",
                columns: table => new
                {
                    IdFerramenta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Checkbox = table.Column<bool>(type: "boolean", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    QuantidadeDisp = table.Column<int>(type: "integer", nullable: false),
                    TipoManutencaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FerramentasTipoModel", x => x.IdFerramenta);
                    table.ForeignKey(
                        name: "FK_FerramentasTipoModel_TipoManutencaoModel_TipoManutencaoId",
                        column: x => x.TipoManutencaoId,
                        principalTable: "TipoManutencaoModel",
                        principalColumn: "IdTipoManutencao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemRenovacaoTipoModel",
                columns: table => new
                {
                    IdItemRenovacaoTipo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoItem = table.Column<string>(type: "text", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    Template1 = table.Column<string>(type: "text", nullable: true),
                    Template2 = table.Column<string>(type: "text", nullable: true),
                    TipoManutencaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRenovacaoTipoModel", x => x.IdItemRenovacaoTipo);
                    table.ForeignKey(
                        name: "FK_ItemRenovacaoTipoModel_TipoManutencaoModel_TipoManutencaoId",
                        column: x => x.TipoManutencaoId,
                        principalTable: "TipoManutencaoModel",
                        principalColumn: "IdTipoManutencao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensPlanoManutencao",
                columns: table => new
                {
                    IdItemPlano = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoItem = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", nullable: true),
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
                    NumeroNota = table.Column<long>(type: "bigint", nullable: false),
                    TipoNota = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescricaoDetalhada = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    StatusNota = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DataNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Prioridade = table.Column<int>(type: "integer", nullable: false),
                    InicioDesejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ConclusaoDesejada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EquipamentoTag = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    LocalInstalacaoCodigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    CriadoPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    TipoPrioridade = table.Column<string>(type: "text", nullable: true),
                    TipoDeNota = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodific = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCodif = table.Column<string>(type: "text", nullable: true),
                    Codificacao = table.Column<string>(type: "text", nullable: true),
                    Consequencia = table.Column<int>(type: "integer", nullable: true),
                    GrupoDeCodes = table.Column<string>(type: "text", nullable: true)
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
                    TipoOrdem = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusOrdem = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NotaRelacionadaId = table.Column<long>(type: "bigint", nullable: true),
                    PlanoManutencaoCodigo = table.Column<long>(type: "bigint", maxLength: 40, nullable: true),
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
                name: "IX_AtividadesNota_NotaId",
                table: "AtividadesNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_CentrosTrabalho_Codigo",
                table: "CentrosTrabalho",
                column: "Codigo",
                unique: true);

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
                name: "IX_FerramentasTipoModel_TipoManutencaoId",
                table: "FerramentasTipoModel",
                column: "TipoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_GruposPlanejamentoManutencao_Codigo",
                table: "GruposPlanejamentoManutencao",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemRenovacaoTipoModel_TipoManutencaoId",
                table: "ItemRenovacaoTipoModel",
                column: "TipoManutencaoId");

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
                name: "IX_OrdensPetro_IdNota",
                table: "OrdensPetro",
                column: "IdNota",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanosManutencaoNormalizados_EstrategiaId",
                table: "PlanosManutencaoNormalizados",
                column: "EstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoEstrategiaModel_EstrategiaId",
                table: "TipoEstrategiaModel",
                column: "EstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoManutencaoModel_PlanoManutencaoId",
                table: "TipoManutencaoModel",
                column: "PlanoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoManutencaoModel_TipoEstrategiaId",
                table: "TipoManutencaoModel",
                column: "TipoEstrategiaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSap_Nome_Mandante",
                table: "UsuariosSap",
                columns: new[] { "Nome", "Mandante" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcaoNotaCongas");

            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "AtividadesNota");

            migrationBuilder.DropTable(
                name: "CiclosManutencao");

            migrationBuilder.DropTable(
                name: "FerramentasTipoModel");

            migrationBuilder.DropTable(
                name: "ItemRenovacaoTipoModel");

            migrationBuilder.DropTable(
                name: "ItensNotaManutencao");

            migrationBuilder.DropTable(
                name: "ItensPlanoManutencao");

            migrationBuilder.DropTable(
                name: "MedidasNota");

            migrationBuilder.DropTable(
                name: "OperacoesOrdem");

            migrationBuilder.DropTable(
                name: "OrdensPetro");

            migrationBuilder.DropTable(
                name: "Regras");

            migrationBuilder.DropTable(
                name: "TipoManutencaoModel");

            migrationBuilder.DropTable(
                name: "OrdensManutencao");

            migrationBuilder.DropTable(
                name: "NotasPetro");

            migrationBuilder.DropTable(
                name: "TipoEstrategiaModel");

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
        }
    }
}
