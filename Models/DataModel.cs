using CsvHelper.Configuration.Attributes;

namespace teste_catboost_cs;

public class ModelMeta
{
    public required List<string> cat_features { get; init; }
    public required List<string> text_features { get; init; }
    public required List<string> features { get; init; }

    /*
    public SortedSet<string> FeatureSet
    {
        get
        {
            field ??= new SortedSet<string>(features);

            return field;
        }
    }
    */
}

public class ItemRow
{
    [Name("Nota")] public int NotaId { get; set; }

    [Name("Nº de ordenação")] public int NumOrdenacao { get; set; }

    // [Name("GrpCódigos")]
    // public string GrpCodigos { get; set; }
    [Name("Codificação")] public string? Codificacao { get; set; }
    // [Name("Code parte obj.")] public string? CodeParteObj { get; set; }
    // [Name("Código do dano")] public string? CodigoDoDano { get; set; }
    // [Name("Cód.causa")] public string? CodCausa { get; set; }
    [Name("Txt.code codif.")] public string? TxtCodeCodif { get; set; }
    [Name("TxtGrpCodPartOb")] public string? TxtGrpCodPartOb { get; set; }
    [Name("Txt.cod.part.ob")] public string? TxtCodPartOb { get; set; }
    [Name("Txt.grp.cod.pr.")] public string? TxtGrpCodPr { get; set; }
    [Name("Txt.code prob.")] public string? TxtCodeProb { get; set; }
    [Name("Texto grp.causa")] public string? TextoGrpCausa { get; set; }
    [Name("Txt.code causa")] public string? TxtCodeCausa { get; set; }
    // [Name("Texto da causa")] public string? TextoDaCausa { get; set; }
    [Name("Grupo de codes")] public string? GrupoDeCodes { get; set; }
    // [Name("Item")] public string? Item { get; set; }
    [Name("Txt.grp.codific")] public string? TxtGrpCodific { get; set; }
    // [Name("Txt.descritivo")] public string? TxtDescritivo { get; set; }
    // [Name("Texto")] public string? Texto { get; set; }
}

public class NotaRow
{
    [Name("Nota")] public int Nota { get; set; }
    [Name("Tipo de nota")] public string? TipoDeNota { get; set; }
    //[Name("Loc.instalação")] public string? LocInstalacao { get; set; }
    //[Name("Status sistema")] public string? StatusSistema { get; set; }
    //[Name("Status usuário")] public string? StatusUsuario { get; set; }
    [Name("Descrição")] public string? Descricao { get; set; }
    //[Name("Rua")] public string? Rua { get; set; }
    //[Name("Bairro")] public string? Bairro { get; set; }
    //[Name("Local")] public string? Local { get; set; }
    // [Name("Data da nota")] public string? DataDaNota { get; set; }
    // [Name("Modificado em")] public string? ModificadoEm { get; set; }
    // [Name("Modificado por")] public string? ModificadoPor { get; set; }
    // [Name("Modificado às")] public string? ModificadoAs { get; set; }
    [Name("Tipo prioridade")] public string? TipoPrioridade { get; set; }
    // [Name("InícioAvar")] public string? InicioAvar { get; set; }
    // [Name("Fim avaria")] public string? FimAvaria { get; set; }
    // [Name("HoraInícioAvar.")] public string? HoraInicioAvar { get; set; }
    // [Name("Hora fim avaria")] public string? HoraFimAvaria { get; set; }
    // [Name("Dt.referência")] public string? DtReferencia { get; set; }
    // [Name("Hora referência")] public string? HotaReferencia { get; set; }
    // [Name("Parada")] public string? Parada { get; set; }
    // [Name("Duraç.parada")] public string? DuracParada { get; set; }
    // [Name("Dt.criação")] public string? DtCriacao { get; set; }
    // [Name("Criado por")] public string? CriadoPor { get; set; }
    // [Name("Criado às")] public string? CriadoAs { get; set; }
    // [Name("Notificador")] public string? Notificador { get; set; }
    // [Name("Perf.catálogo")] public string? PerfCatalogo { get; set; }
    // [Name("CenTrab respon.")] public string? CenTrabRespon { get; set; }
    [Name("Txt.code codif.")] public string? TxtCodeCodif { get; set; }
    [Name("Txt.grp.codific")] public string? TxtGrpCodific { get; set; }
    // [Name("Loc.inst.afetad")] public string? LocInstAfetad { get; set; }
    [Name("Equipamento")] public string? Equipamento { get; set; }
    // [Name("Denominação")] public string? Denominacao { get; set; }
    // [Name("Grp.plnj.PM")] public string? GrpPlnjPM { get; set; }
    // [Name("Área operacion.")] public string? AreaOperacion { get; set; }
    [Name("Codificação")] public string? CodificacaoX { get; set; }
    [Name("Conseqüência")] public string? Consequencia { get; set; }
    [Name("Grupo de codes")] public string? GrupoDeCodesX { get; set; }
    // [Name("Ordem")] public string? Ordem { get; set; }
}