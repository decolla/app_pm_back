using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de item de nota de manutencao Comgas.
/// </summary>
public class ItemNotaManutencaoComgasDto
{
    [SheetColumn("Id Item")]
    public int IdItemNota { get; set; }

    [SheetColumn("Nota")]
    public long NotaId { get; set; }

    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    [SheetColumn("Code parte obj.")]
    public string? ParteObjeto { get; set; }

    [SheetColumn("Sintoma")]
    public string? Sintoma { get; set; }

    [SheetColumn("Txt.code prob.")]
    public string? TextoSintoma { get; set; }

    [SheetColumn("Causa")]
    public string? Causa { get; set; }

    [SheetColumn("Texto da causa")]
    public string? TextoCausa { get; set; }

    [SheetColumn("Nº de ordenação")]
    public string? NDeOrdenacao1 { get; set; }

    [SheetColumn("TxtGrpCodPartOb")]
    public string? TxtGrpCodPartOb { get; set; }

    [SheetColumn("Txt.cod.part.ob")]
    public string? TxtCodPartOb { get; set; }

    [SheetColumn("Txt.code prob.")]
    public string? TxtCodeProb { get; set; }

    [SheetColumn("Texto grp.causa")]
    public string? TextoGrpCausa { get; set; }

    [SheetColumn("Txt.grp.cod.pr.")]
    public string? TxtGrpCodPr { get; set; }

    [SheetColumn("Txt.code causa")]
    public string? TxtCodeCausa { get; set; }
}

#pragma warning restore CS1591
