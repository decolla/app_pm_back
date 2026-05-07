using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    /// Nome do grupo.
    /// </summary>
    public string Nome { get; set; }
    
    /// <summary>
    /// Centro de planejamento associado ao grupo.
    /// </summary>
    public int? IdCpm { get; set; }
    
    /// <summary>
    /// Status do grupo.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Descricao do grupo.
    /// </summary>
    [MaxLength(150)]
    public string? Descricao { get; set; }
    
    /// <summary>
    /// Centro de planejamento associado ao grupo.
    /// </summary>
    [ForeignKey(nameof(IdCpm))]
    public virtual CentroPlanejamentoManutencaoModel? CentroPlanejamentoManutencao { get; set; } = null!;

    /// <summary>
    /// Locais de instalacao vinculados ao grupo.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();
    
    /// <summary>
    /// Funcionarios vinculados ao grupo.
    /// </summary>
    public virtual ICollection<FuncionarioModel> ListaFuncionario { get; set; } = new List<FuncionarioModel>();
}
