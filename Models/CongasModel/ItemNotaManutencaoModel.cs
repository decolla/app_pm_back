using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    /// <summary>
    /// Nota associada ao item.
    /// </summary>
    [ForeignKey(nameof(NotaId))]
    public virtual NotaManutencaoModel Nota { get; set; } = null!;
}

