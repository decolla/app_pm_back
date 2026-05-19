using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Componente vinculado a um equipamento.
/// </summary>
public class ComponenteEquipamentoModel
{
    /// <summary>
    /// Identificador interno do componente.
    /// </summary>
    [Key]
    public int IdComponente { get; set; }

    /// <summary>
    /// TAG do equipamento associado.
    /// </summary>
    [MaxLength(60)]
    public string? EquipamentoTag { get; set; }

    /// <summary>
    /// Denominacao do equipamento (para exibicao).
    /// </summary>
    [MaxLength(200)]
    public string? DenominacaoEquipamento { get; set; }

    /// <summary>
    /// Identificador do componente no equipamento.
    /// </summary>
    [MaxLength(60)]
    public string? CodigoComponente { get; set; }

    /// <summary>
    /// Nome do componente.
    /// </summary>
    [MaxLength(150)]
    public string? NomeComponente { get; set; }

    /// <summary>
    /// Quantidade do componente.
    /// </summary>
    public decimal? Quantidade { get; set; }

    /// <summary>
    /// Unidade de medida do componente.
    /// </summary>
    [MaxLength(30)]
    public string? UnidadeMedida { get; set; }

    /// <summary>
    /// Data de inicio de validade do componente.
    /// </summary>
    public DateTime? ValidoDesde { get; set; }

    /// <summary>
    /// Data de fim de validade do componente.
    /// </summary>
    public DateTime? ValidoAte { get; set; }

    /// <summary>
    /// Informacao adicional sobre o componente.
    /// </summary>
    [MaxLength(500)]
    public string? InformacaoAdicional { get; set; }

    /// <summary>
    /// Equipamento associado.
    /// </summary>
    [ForeignKey(nameof(EquipamentoTag))]
    public virtual EquipamentoAtivoModel? Equipamento { get; set; }
}

