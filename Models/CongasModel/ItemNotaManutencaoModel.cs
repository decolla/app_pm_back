using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Item de diagnostico de uma nota de manutencao.
/// </summary>
public class ItemNotaManutencaoModel
{
    /// <summary>
    /// Identificador do item.
    /// </summary>
    [Key]
    public int IdItemNota { get; set; }

    /// <summary>
    /// Nota dona do item.
    /// </summary>
    public long NotaId { get; set; }

    /// <summary>
    /// Parte objeto reportada.
    /// </summary>
    [MaxLength(120)]
    public string? ParteObjeto { get; set; }

    /// <summary>
    /// Codigo do sintoma.
    /// </summary>
    [MaxLength(30)]
    public string? Sintoma { get; set; }

    /// <summary>
    /// Texto explicativo do sintoma.
    /// </summary>
    [MaxLength(500)]
    public string? TextoSintoma { get; set; }

    /// <summary>
    /// Codigo da causa.
    /// </summary>
    [MaxLength(30)]
    public string? Causa { get; set; }

    /// <summary>
    /// Texto explicativo da causa.
    /// </summary>
    [MaxLength(500)]
    public string? TextoCausa { get; set; }
    
    // CATBOOST

    /// <summary>
    /// Número de ordenação do item
    /// </summary>
    [SheetColumn("Nº de ordenação")]
    public string? NDeOrdenacao1 { get; set; }

    /// <summary>
    /// Texto do grupo do código da parte do objeto do item
    /// </summary>
    [SheetColumn("TxtGrpCodPartOb")]
    public string? TxtGrpCodPartOb { get; set; }

    /// <summary>
    /// Texto do código da parte do objeto do item
    /// </summary>
    [SheetColumn("Txt.cod.part.ob")]
    public string? TxtCodPartOb { get; set; }


    /// <summary>
    /// Texto do código de problema do item
    /// </summary>
    [SheetColumn("Txt.code prob.")]
    public string? TxtCodeProb { get; set; }

    /// <summary>
    /// Texto do grupo da causa do item
    /// </summary>
    [SheetColumn("Texto grp.causa")]
    public string? TextoGrpCausa { get; set; }

    /// <summary>
    /// Texto do grupo do código de problema do item
    /// </summary>
    [SheetColumn("Txt.grp.cod.pr.")]
    public string? TxtGrpCodPr { get; set; }

    /// <summary>
    /// Texto do código da causa do item
    /// </summary>
    [SheetColumn("Txt.code causa")]
    public string? TxtCodeCausa { get; set; }

    /// <summary>
    /// Nota associada ao item.
    /// </summary>
    [ForeignKey(nameof(NotaId))]
    public virtual NotaManutencaoModel Nota { get; set; } = null!;
}

