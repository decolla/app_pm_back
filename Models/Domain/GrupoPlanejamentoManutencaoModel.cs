using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Grupo de planejamento de manutencao (GPM).
/// </summary>
public class GrupoPlanejamentoManutencaoModel
{
    /// <summary>
    /// Identificador interno do grupo.
    /// </summary>
    [Key]
    public int IdGpm { get; set; }

    /// <summary>
    /// Codigo do grupo no SAP.
    /// </summary>
    [MaxLength(30)]
    public required string Codigo { get; set; }

    /// <summary>
    /// Descricao do grupo.
    /// </summary>
    [MaxLength(150)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Locais de instalacao vinculados ao grupo.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();

    /// <summary>
    /// Equipamentos vinculados ao grupo.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();
}

