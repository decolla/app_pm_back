using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

public class TarefaModel
{
    /// <summary>
    /// Id da tarefa
    /// </summary>
    [Key]
    public int IdTarefa { get; set; }
    
    /// <summary>
    /// Nome da tarefa
    /// </summary>
    public string Nome { get; set; }
    
    /// <summary>
    /// Descricao da tarefa
    /// </summary>
    public string Descricao { get; set; }
    
    /// <summary>
    /// Duracao da tarefa em horas.
    /// </summary>
    public int Duracao { get; set; }
    
    /// <summary>
    /// FK do tipo de manutencao.
    /// </summary>
    public int TipoManutencaoId { get; set; }
    [ForeignKey(nameof(TipoManutencaoId))]
    public virtual TipoManutencaoModel TipoManutencao { get; set; } = null!;
}