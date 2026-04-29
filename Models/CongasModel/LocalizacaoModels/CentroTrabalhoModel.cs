using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    /// Nome do centro de trabalho.
    /// </summary>
    [MaxLength(150)]
    public string? Nome { get; set; }

    /// <summary>
    /// Identificador do centro de planejamento (CPM).
    /// </summary>
    public int? CpmId { get; set; }

    /// <summary>
    /// Status do centro de trabalho.
    /// </summary>
    [MaxLength(30)]
    public string? Status { get; set; }

    /// <summary>
    /// Descricao do centro de trabalho.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Codigo do centro de trabalho.
    /// </summary>
    [MaxLength(40)]
    public string? Codigo { get; set; }

    /// <summary>
    /// Centro de planejamento associado.
    /// </summary>
    [ForeignKey(nameof(CpmId))]
    public virtual CentroPlanejamentoManutencaoModel? CentroPlanejamentoManutencao { get; set; }

    /// <summary>
    /// Locais de instalacao com este centro de trabalho.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();

    /// <summary>
    /// Equipamentos com este centro de trabalho.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();

    /// <summary>
    /// Funcionarios vinculados ao centro de trabalho.
    /// </summary>
    public virtual ICollection<FuncionarioModel> Funcionarios { get; set; } = new List<FuncionarioModel>();
}
