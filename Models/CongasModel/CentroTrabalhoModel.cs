using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Centro de trabalho responsavel por execucao e planejamento.
/// </summary>
public class CentroTrabalhoModel
{
    /// <summary>
    /// Identificador interno do centro de trabalho.
    /// </summary>
    [Key]
    public int IdCentroTrabalho { get; set; }

    /// <summary>
    /// Codigo do centro de trabalho.
    /// </summary>
    [MaxLength(30)]
    public required string Codigo { get; set; }

    /// <summary>
    /// Nome do centro de trabalho.
    /// </summary>
    [MaxLength(150)]
    public string? Nome { get; set; }

    /// <summary>
    /// Centro de planejamento associado.
    /// </summary>
    [MaxLength(30)]
    public string? CentroPlanejamento { get; set; }

    /// <summary>
    /// Locais de instalacao com este centro de trabalho.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();

    /// <summary>
    /// Equipamentos com este centro de trabalho.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();
}

