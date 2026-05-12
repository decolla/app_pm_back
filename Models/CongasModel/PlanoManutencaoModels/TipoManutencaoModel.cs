using System.ComponentModel.DataAnnotations;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

namespace app_pm_back.Models.Domain.PlanoManutencaoModels;

public class TipoManutencaoModel
{
    
    /// <summary>
    /// Identificador interno do tipo de manutencao.
    /// </summary>
    [Key]
    public int IdTipoManutencao { get; set; }
    
    /// <summary>
    /// Tipo do tipo de manutencao, se é uma manutencao ou renovacao de ativo.
    /// </summary>
    /// <example>Manutencao = 0, RenovacaoAtivo = 1</example>
    public TipoTipo Categoria { get; set; }
    
    /// <summary>
    /// Nome do tipo de manutencao.
    /// </summary>
    public string Nome { get; set; }
    
    /// <summary>
    /// Tipo de ativo suportado pelo tipo de manutencao.
    /// </summary>
    public string TipoAtivo { get; set; } 
    
    /// <summary>
    /// Confiabilidade do tipo de manutencao em porcentagem.
    /// </summary>
    [Range(0, 100)]
    public int Confiabilidade { get; set; }
    
    /// <summary>
    /// Se o tipo de manutencao esta habilitado para ser utilizado em planos de manutencao.
    /// </summary>
    public bool Habilitada { get; set; }
    
    /// <summary>
    /// Guarda fas tarefas associadas a este tipo de manutencao.
    /// </summary>
    public virtual ICollection<TarefaModel>? ListaTarefas { get; set; } = new List<TarefaModel>();
    
    /// <summary>
    /// Guarda os tipos de estrategia associados a este tipo de manutencao.
    /// </summary>
    public virtual ICollection<TipoEstrategiaModel>? TiposEstrategia { get; set; } = new List<TipoEstrategiaModel>();
    
    /// <summary>
    /// Guarda os tipos de plano associados a este tipo de manutencao.
    /// </summary>
    public virtual ICollection<TipoPlanoModel>? TipoPlanoModels { get; set; } = new List<TipoPlanoModel>();
}

/// <summary>
/// Qual o tipo do tipo cadastrado.
/// </summary>
public enum TipoTipo
{
    Manutencao = 0,
    RenovacaoAtivo = 1
}