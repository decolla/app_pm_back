using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Item de manutencao pertencente a um plano.
/// </summary>
public class ItemPlanoManutencaoModel
{
    /// <summary>
    /// Identificador do item.
    /// </summary>
    [Key]
    public int IdItemPlano { get; set; }

    /// <summary>
    /// Codigo do item de manutencao.
    /// </summary>
    [MaxLength(40)]
    public string? CodigoItem { get; set; }

    /// <summary>
    /// Codigo do plano de manutencao.
    /// </summary>
    public long? PlanoManutencaoCodigo { get; set; }

    /// <summary>
    /// TAG do equipamento relacionado.
    /// </summary>
    [MaxLength(60)]
    public string? EquipamentoTag { get; set; }

    /// <summary>
    /// Codigo do local de instalacao relacionado.
    /// </summary>
    [MaxLength(60)]
    public string? LocalInstalacaoCodigo { get; set; }

    /// <summary>
    /// Plano de manutencao dono do item.
    /// </summary>
    [ForeignKey(nameof(PlanoManutencaoCodigo))]
    public virtual PlanoManutencaoModel PlanoManutencao { get; set; } = null!;

    /// <summary>
    /// Equipamento relacionado ao item.
    /// </summary>
    [ForeignKey(nameof(EquipamentoTag))]
    public virtual EquipamentoAtivoModel? Equipamento { get; set; }

    /// <summary>
    /// Local de instalacao relacionado ao item.
    /// </summary>
    [ForeignKey(nameof(LocalInstalacaoCodigo))]
    public virtual LocalInstalacaoModel? LocalInstalacao { get; set; }
}

