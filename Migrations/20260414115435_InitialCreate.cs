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
                name: "Equipamentos",
                columns: table => new
                {
                    IdEquipamento = table.Column<string>(type: "text", nullable: false),
                    Atividade = table.Column<string>(type: "text", nullable: true),
                    BaseDeAtendiemnto = table.Column<string>(type: "text", nullable: true),
                    Capacidade = table.Column<string>(type: "text", nullable: true),
                    Classe = table.Column<string>(type: "text", nullable: true),
                    CoordenadasX = table.Column<string>(type: "text", nullable: true),
                    CoordenadasY = table.Column<string>(type: "text", nullable: true),
                    DataLoggerId = table.Column<string>(type: "text", nullable: true),
                    DDD = table.Column<string>(type: "text", nullable: true),
                    Denominacao = table.Column<string>(type: "text", nullable: true),
                    ElementoPep = table.Column<string>(type: "text", nullable: true),
                    Fabricante = table.Column<string>(type: "text", nullable: true),
                    FrequenciaDeManutencao = table.Column<string>(type: "text", nullable: true),
                    Grid = table.Column<string>(type: "text", nullable: true),
                    IdGeogas = table.Column<string>(type: "text", nullable: true),
                    Iluminacao = table.Column<string>(type: "text", nullable: true),
                    Material = table.Column<string>(type: "text", nullable: true),
                    NumeroDaEstacaoInterligada = table.Column<string>(type: "text", nullable: true),
                    NumeroDeLinhas = table.Column<int>(type: "integer", nullable: true),
                    NumeroDoProjeto = table.Column<string>(type: "text", nullable: true),
                    PontoDeReferencia = table.Column<string>(type: "text", nullable: true),
                    Secao = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Subsistema = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    TipoDeObjeto = table.Column<string>(type: "text", nullable: true),
                    DataDeAssentamento = table.Column<string>(type: "text", nullable: true),
                    DataDeComissionamento = table.Column<string>(type: "text", nullable: true),
                    DataDeFabricacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataDeInicioDeOper = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataDeInstalacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataFimDeOperacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DetalhesTecnicos = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipamentos", x => x.IdEquipamento);
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
                name: "OrdensCongas",
                columns: table => new
                {
                    IdOrdem = table.Column<long>(type: "bigint", nullable: false),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LocInstalacao = table.Column<string>(type: "text", nullable: false),
                    AreaContCsts = table.Column<string>(type: "text", nullable: true),
                    AreaOperacion = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    CampoOrdenacao = table.Column<string>(type: "text", nullable: true),
                    CanalDistrib = table.Column<string>(type: "text", nullable: true),
                    CanalDistribVen = table.Column<string>(type: "text", nullable: true),
                    CategoriaOrdem = table.Column<string>(type: "text", nullable: true),
                    CenCstRespons = table.Column<string>(type: "text", nullable: true),
                    Centro = table.Column<string>(type: "text", nullable: true),
                    CentroCusto = table.Column<string>(type: "text", nullable: true),
                    CentroDeLucro = table.Column<string>(type: "text", nullable: true),
                    CentroTrabalho = table.Column<string>(type: "text", nullable: true),
                    CenLocaliz = table.Column<string>(type: "text", nullable: true),
                    CentroPlanej = table.Column<string>(type: "text", nullable: true),
                    CenTrabRespon = table.Column<string>(type: "text", nullable: true),
                    ChaveSobretaxa = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    Cliente = table.Column<string>(type: "text", nullable: true),
                    CntrCntrTrab = table.Column<string>(type: "text", nullable: true),
                    CodigoABC = table.Column<string>(type: "text", nullable: true),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true),
                    CodPlanOrdem = table.Column<string>(type: "text", nullable: true),
                    CondInstal = table.Column<string>(type: "text", nullable: true),
                    Conjunto = table.Column<string>(type: "text", nullable: true),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    CriterioParaCalc = table.Column<string>(type: "text", nullable: true),
                    CustsEstimados = table.Column<decimal>(type: "numeric", nullable: true),
                    CustosTotPlan = table.Column<decimal>(type: "numeric", nullable: true),
                    CustTotReais = table.Column<decimal>(type: "numeric", nullable: true),
                    TotLiquidacao = table.Column<decimal>(type: "numeric", nullable: true),
                    DataBaseFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataBaseInic = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataDeEntrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraDeInicio = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DataModif = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtReferencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPedido = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtRealFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LiberacaoReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DdsDispositivo = table.Column<string>(type: "text", nullable: true),
                    DenomConjunto = table.Column<string>(type: "text", nullable: true),
                    Denominacao = table.Column<string>(type: "text", nullable: true),
                    ProdServico = table.Column<string>(type: "text", nullable: true),
                    DispDesdeData = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DispDesdeHora = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DisponivelAte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DispAteHora = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Divisao = table.Column<string>(type: "text", nullable: true),
                    DocVendas = table.Column<string>(type: "text", nullable: true),
                    DtaInicioReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ElementoPEP = table.Column<string>(type: "text", nullable: true),
                    ElemRefPMPS = table.Column<string>(type: "text", nullable: true),
                    EmGarantia = table.Column<string>(type: "text", nullable: true),
                    Empresa = table.Column<string>(type: "text", nullable: true),
                    Equipamento = table.Column<string>(type: "text", nullable: true),
                    EquipeVendas = table.Column<string>(type: "text", nullable: true),
                    EscritVendas = table.Column<string>(type: "text", nullable: true),
                    EsquemaCalculo = table.Column<string>(type: "text", nullable: true),
                    ExisteTxtDesc = table.Column<string>(type: "text", nullable: true),
                    FimBaseHora = table.Column<TimeSpan>(type: "interval", nullable: true),
                    FimProgramado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraFim = table.Column<TimeSpan>(type: "interval", nullable: true),
                    FimRealHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FormaFatmto = table.Column<string>(type: "text", nullable: true),
                    GrpPlnManLTa = table.Column<string>(type: "text", nullable: true),
                    GrpPlnjPM = table.Column<string>(type: "text", nullable: true),
                    GrpLisTar = table.Column<string>(type: "text", nullable: true),
                    HistoricoDesde = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraReferencia = table.Column<TimeSpan>(type: "interval", nullable: true),
                    InicRealHr = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Imobilizado = table.Column<string>(type: "text", nullable: true),
                    InicioProg = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Item = table.Column<string>(type: "text", nullable: true),
                    ItemManut = table.Column<long>(type: "bigint", nullable: true),
                    NrListaObj = table.Column<string>(type: "text", nullable: true),
                    Local = table.Column<string>(type: "text", nullable: true),
                    Localizacao = table.Column<string>(type: "text", nullable: true),
                    MarcPElimin = table.Column<string>(type: "text", nullable: true),
                    Material = table.Column<string>(type: "text", nullable: true),
                    Moeda = table.Column<string>(type: "text", nullable: true),
                    NrDeSerie = table.Column<string>(type: "text", nullable: true),
                    NrDoPedido = table.Column<string>(type: "text", nullable: true),
                    NrEndereco = table.Column<string>(type: "text", nullable: true),
                    NomeLista = table.Column<string>(type: "text", nullable: true),
                    NrObjeto = table.Column<string>(type: "text", nullable: true),
                    NumeroNotaOrigem = table.Column<long>(type: "bigint", nullable: true),
                    NumdrGrpRot = table.Column<string>(type: "text", nullable: true),
                    OperacSuperior = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<string>(type: "text", nullable: true),
                    OrdemPrincip = table.Column<string>(type: "text", nullable: true),
                    OrdemSuperior = table.Column<string>(type: "text", nullable: true),
                    OrgVendas = table.Column<string>(type: "text", nullable: true),
                    OrgVendasVendas = table.Column<string>(type: "text", nullable: true),
                    PEPCabecOrdem = table.Column<string>(type: "text", nullable: true),
                    PlanoManut = table.Column<long>(type: "bigint", nullable: true),
                    Prioridade = table.Column<string>(type: "text", nullable: true),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: true),
                    RecTotaisPlan = table.Column<decimal>(type: "numeric", nullable: true),
                    RecTotReais = table.Column<decimal>(type: "numeric", nullable: true),
                    RedeParcialP = table.Column<string>(type: "text", nullable: true),
                    Regiao = table.Column<string>(type: "text", nullable: true),
                    ReservaReqComp = table.Column<string>(type: "text", nullable: true),
                    Revisao = table.Column<string>(type: "text", nullable: true),
                    Rua = table.Column<string>(type: "text", nullable: true),
                    SetorAtividade = table.Column<string>(type: "text", nullable: true),
                    SetorAtividadeVe = table.Column<string>(type: "text", nullable: true),
                    StatusPaging = table.Column<string>(type: "text", nullable: true),
                    StatusSistema = table.Column<string>(type: "text", nullable: true),
                    StatusUsuario = table.Column<string>(type: "text", nullable: true),
                    Subnr = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    TextoBreve = table.Column<string>(type: "text", nullable: false),
                    TipoAtividPM = table.Column<string>(type: "text", nullable: true),
                    TipoOrdem = table.Column<string>(type: "text", nullable: false),
                    TipoPrioridade = table.Column<string>(type: "text", nullable: true),
                    TotalPlanejado = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalReal = table.Column<decimal>(type: "numeric", nullable: true),
                    ModificadoPor = table.Column<string>(type: "text", nullable: true),
                    UMBasica = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensCongas", x => x.IdOrdem);
                });

            migrationBuilder.CreateTable(
                name: "PlanosManutnecao",
                columns: table => new
                {
                    IdPlano = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanoManut = table.Column<long>(type: "bigint", nullable: true),
                    Ciclo = table.Column<int>(type: "integer", nullable: false),
                    Unidade = table.Column<string>(type: "text", nullable: false),
                    TxtCicloManut = table.Column<string>(type: "text", nullable: true),
                    Offset = table.Column<int>(type: "integer", nullable: true),
                    ItemManut = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanosManutnecao", x => x.IdPlano);
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
                name: "NotasCongas",
                columns: table => new
                {
                    IdNota = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroNota = table.Column<long>(type: "bigint", nullable: false),
                    TipoNota = table.Column<string>(type: "text", nullable: true),
                    AreaOperac = table.Column<int>(type: "integer", nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    MetodoClassificacao = table.Column<string>(type: "text", nullable: true),
                    ClassificadoPor = table.Column<string>(type: "text", nullable: true),
                    DataClassificacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LocInstalacao = table.Column<string>(type: "text", nullable: true),
                    StatusSistema = table.Column<string>(type: "text", nullable: true),
                    StatusUsuario = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Rua = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Local = table.Column<string>(type: "text", nullable: true),
                    DataNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificadoPor = table.Column<string>(type: "text", nullable: true),
                    ModificadoAs = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoPrioridade = table.Column<string>(type: "text", nullable: true),
                    InicioAvar = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraInicioAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraFimAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtReferencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraReferencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Parada = table.Column<string>(type: "text", nullable: true),
                    DuracParada = table.Column<decimal>(type: "numeric", nullable: true),
                    DtCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    CriadoAs = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notificador = table.Column<string>(type: "text", nullable: true),
                    PerfCatalogo = table.Column<string>(type: "text", nullable: true),
                    CenTrabRespon = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCodif = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodific = table.Column<string>(type: "text", nullable: true),
                    LocInstAfetado = table.Column<string>(type: "text", nullable: true),
                    Equipamento = table.Column<string>(type: "text", nullable: true),
                    Denominacao = table.Column<string>(type: "text", nullable: true),
                    GrpPlnjPM = table.Column<string>(type: "text", nullable: true),
                    AreaOperacion = table.Column<string>(type: "text", nullable: true),
                    Codificacao = table.Column<string>(type: "text", nullable: true),
                    Consequencia = table.Column<int>(type: "integer", nullable: true),
                    GrupoCodes = table.Column<string>(type: "text", nullable: true),
                    IdOrdem = table.Column<long>(type: "bigint", nullable: true),
                    AreaContCsts = table.Column<string>(type: "text", nullable: true),
                    CampoOrdenacao = table.Column<string>(type: "text", nullable: true),
                    CanalDistrib = table.Column<string>(type: "text", nullable: true),
                    CentroCusto = table.Column<string>(type: "text", nullable: true),
                    CenLocaliz = table.Column<string>(type: "text", nullable: true),
                    CentroParaCenTrb = table.Column<string>(type: "text", nullable: true),
                    CenParaCenTrab = table.Column<string>(type: "text", nullable: true),
                    CentroPlanej = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    Cliente = table.Column<string>(type: "text", nullable: true),
                    CodigoABC = table.Column<string>(type: "text", nullable: true),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true),
                    ConclDesejada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CondInstAntAvar = table.Column<string>(type: "text", nullable: true),
                    CondInstDepAvar = table.Column<string>(type: "text", nullable: true),
                    CondInstAposMed = table.Column<string>(type: "text", nullable: true),
                    Conjunto = table.Column<string>(type: "text", nullable: true),
                    ControlTecnEm = table.Column<string>(type: "text", nullable: true),
                    ContrTecnDe = table.Column<string>(type: "text", nullable: true),
                    DdsDispositivo = table.Column<string>(type: "text", nullable: true),
                    DenomConjunto = table.Column<string>(type: "text", nullable: true),
                    DenomExecutor = table.Column<string>(type: "text", nullable: true),
                    DispAntesAvaria = table.Column<string>(type: "text", nullable: true),
                    DispAposMed = table.Column<string>(type: "text", nullable: true),
                    DispDepoisAvar = table.Column<string>(type: "text", nullable: true),
                    Divisao = table.Column<string>(type: "text", nullable: true),
                    DocVendas = table.Column<string>(type: "text", nullable: true),
                    EmGarantia = table.Column<string>(type: "text", nullable: true),
                    Empresa = table.Column<string>(type: "text", nullable: true),
                    DataEncermto = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EquipamAfetado = table.Column<string>(type: "text", nullable: true),
                    EquipeVendas = table.Column<string>(type: "text", nullable: true),
                    EscritVendas = table.Column<string>(type: "text", nullable: true),
                    ExecutadoPor = table.Column<string>(type: "text", nullable: true),
                    HoraDaNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraEncermto = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraInicDes = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HFinalDesej = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InicioDesejado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IDRamal = table.Column<string>(type: "text", nullable: true),
                    Imobilizado = table.Column<string>(type: "text", nullable: true),
                    Instalacao = table.Column<string>(type: "text", nullable: true),
                    Item = table.Column<string>(type: "text", nullable: true),
                    ItemManut = table.Column<long>(type: "bigint", nullable: true),
                    Localizacao = table.Column<string>(type: "text", nullable: true),
                    LocalClasContb = table.Column<string>(type: "text", nullable: true),
                    Material = table.Column<string>(type: "text", nullable: true),
                    Moeda = table.Column<string>(type: "text", nullable: true),
                    NumeroSerie = table.Column<string>(type: "text", nullable: true),
                    NumeroCertificado = table.Column<string>(type: "text", nullable: true),
                    NumeroPedido = table.Column<string>(type: "text", nullable: true),
                    DataPedido = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NumeroEndereco = table.Column<string>(type: "text", nullable: true),
                    NomeLista = table.Column<string>(type: "text", nullable: true),
                    NumeroObjeto = table.Column<string>(type: "text", nullable: true),
                    OrdemCliente = table.Column<string>(type: "text", nullable: true),
                    OrgVendas = table.Column<string>(type: "text", nullable: true),
                    PEP = table.Column<string>(type: "text", nullable: true),
                    PlanoManut = table.Column<long>(type: "bigint", nullable: true),
                    Prioridade = table.Column<string>(type: "text", nullable: true),
                    Procedencia = table.Column<string>(type: "text", nullable: true),
                    ReferenciaExterna = table.Column<string>(type: "text", nullable: true),
                    Regiao = table.Column<string>(type: "text", nullable: true),
                    Revisao = table.Column<string>(type: "text", nullable: true),
                    Segmento = table.Column<string>(type: "text", nullable: true),
                    SetorAtividade = table.Column<string>(type: "text", nullable: true),
                    StatusPaging = table.Column<string>(type: "text", nullable: true),
                    Subnumero = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    TxtDescritivo = table.Column<string>(type: "text", nullable: true),
                    Unidade = table.Column<string>(type: "text", nullable: true),
                    ZonaAtendimento = table.Column<string>(type: "text", nullable: true),
                    EquipmentModelIdEquipamento = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCongas", x => x.IdNota);
                    table.ForeignKey(
                        name: "FK_NotasCongas_Equipamentos_EquipmentModelIdEquipamento",
                        column: x => x.EquipmentModelIdEquipamento,
                        principalTable: "Equipamentos",
                        principalColumn: "IdEquipamento");
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
                name: "ItemNotaCongas",
                columns: table => new
                {
                    IdItem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoteCongasIdNota = table.Column<int>(type: "integer", nullable: true),
                    AreaOperac = table.Column<int>(type: "integer", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    CanalDistrib = table.Column<string>(type: "text", nullable: true),
                    Causa = table.Column<int>(type: "integer", nullable: true),
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
                    CodCausa = table.Column<string>(type: "text", nullable: true),
                    CodigoDoDano = table.Column<string>(type: "text", nullable: true),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true),
                    ConclDesejada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CondInstAntAvar = table.Column<int>(type: "integer", nullable: true),
                    CondInstDepAvar = table.Column<int>(type: "integer", nullable: true),
                    CondInstAposMed = table.Column<int>(type: "integer", nullable: true),
                    Conjunto = table.Column<string>(type: "text", nullable: true),
                    Consequencia = table.Column<int>(type: "integer", nullable: true),
                    ControlTecnEm = table.Column<string>(type: "text", nullable: true),
                    ContrTecnDe = table.Column<string>(type: "text", nullable: true),
                    CriadoAs = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    DataDaNota = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DtReferencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPedido = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DdsDispositivo = table.Column<string>(type: "text", nullable: true),
                    DenomConjunto = table.Column<string>(type: "text", nullable: true),
                    Denominacao1 = table.Column<string>(type: "text", nullable: true),
                    Denominacao2 = table.Column<string>(type: "text", nullable: true),
                    Denominacao3 = table.Column<string>(type: "text", nullable: true),
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
                    FimAvaria = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GrpCodigos1 = table.Column<string>(type: "text", nullable: true),
                    GrpCodigos2 = table.Column<string>(type: "text", nullable: true),
                    GrpPlnjPM = table.Column<string>(type: "text", nullable: true),
                    GrupoDeCodes = table.Column<string>(type: "text", nullable: true),
                    HoraDaNota = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraEncermto = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraInicDes = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ModificadoAs = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraReferencia = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraFimAvaria = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HFinalDesej = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraInicioAvar = table.Column<TimeSpan>(type: "interval", nullable: true),
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
                    NDeOrdenacao1 = table.Column<string>(type: "text", nullable: true),
                    NDeOrdenacao2 = table.Column<string>(type: "text", nullable: true),
                    NDeSerie = table.Column<string>(type: "text", nullable: true),
                    NDoPedido = table.Column<string>(type: "text", nullable: true),
                    NomeLista = table.Column<string>(type: "text", nullable: true),
                    NumeroNotaOrigem = table.Column<long>(type: "bigint", nullable: false),
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
                    SubN = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Denominacao4 = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCausa = table.Column<string>(type: "text", nullable: true),
                    TxtCodeCodif = table.Column<string>(type: "text", nullable: true),
                    TxtCodPartOb = table.Column<string>(type: "text", nullable: true),
                    TextoDaCausa = table.Column<string>(type: "text", nullable: true),
                    TxtCodeProb = table.Column<string>(type: "text", nullable: true),
                    TextoGrpCausa = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodPr = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodPartOb = table.Column<string>(type: "text", nullable: true),
                    TxtGrpCodific = table.Column<string>(type: "text", nullable: true),
                    TxtDescritivo = table.Column<string>(type: "text", nullable: true),
                    Texto = table.Column<string>(type: "text", nullable: true),
                    Prioridade2 = table.Column<string>(type: "text", nullable: true),
                    TipoDeNota = table.Column<string>(type: "text", nullable: true),
                    TipoPrioridade = table.Column<string>(type: "text", nullable: true),
                    Unidade = table.Column<string>(type: "text", nullable: true),
                    Versao1 = table.Column<string>(type: "text", nullable: true),
                    Versao2 = table.Column<string>(type: "text", nullable: true),
                    Versao3 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemNotaCongas", x => x.IdItem);
                    table.ForeignKey(
                        name: "FK_ItemNotaCongas_NotasCongas_NoteCongasIdNota",
                        column: x => x.NoteCongasIdNota,
                        principalTable: "NotasCongas",
                        principalColumn: "IdNota");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemNotaCongas_NoteCongasIdNota",
                table: "ItemNotaCongas",
                column: "NoteCongasIdNota");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCongas_EquipmentModelIdEquipamento",
                table: "NotasCongas",
                column: "EquipmentModelIdEquipamento");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensPetro_IdNota",
                table: "OrdensPetro",
                column: "IdNota",
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
                name: "ItemNotaCongas");

            migrationBuilder.DropTable(
                name: "OrdensCongas");

            migrationBuilder.DropTable(
                name: "OrdensPetro");

            migrationBuilder.DropTable(
                name: "PlanosManutnecao");

            migrationBuilder.DropTable(
                name: "Regras");

            migrationBuilder.DropTable(
                name: "NotasCongas");

            migrationBuilder.DropTable(
                name: "NotasPetro");

            migrationBuilder.DropTable(
                name: "Equipamentos");
        }
    }
}
