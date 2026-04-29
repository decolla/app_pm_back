using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using app_pm_back.Models.Domain;

namespace app_pm_back.Models.CongasModel;

public class TipoManutencaoModel
{
    /// <summary>
    /// Id do tipo de manutencao.
    /// </summary>
    [Key]
    public int IdTipoManutencao { get; set; } 
    
    /// <summary>
    /// Tipo do tipo a ser cadastrado.
    /// </summary>
    /// <example>Manutencao = 0,
    /// RenovacaoAtivo = 1</example>
    public TipoTipo TipoTipo { get; set; }
    
    /// <summary>
    /// Nome do tipo de manutencao.
    /// </summary>
    [MaxLength(150)]
    public required string NomeTipoManutencao { get; set; }
    
    /// <summary>
    /// Se o tipo está habilitado.
    /// </summary>
    public bool Habilitada { get; set; } = true;
    
    /// <summary>
    /// Confiabilidade do tipo de manutencao em porcentagem.
    /// </summary>
    [Range(0, 100)]
    public int Confiabilidade { get; set; }
    
    /// <summary>
    /// Tipo de ativo.
    /// </summary>
    /// <example>ERP</example>
    [MaxLength(80)]
    public required string TipoAtivo { get; set; }

    /// <summary>
    /// FK do tipo de estratégia.
    /// </summary>
    public int? TipoEstrategiaId { get; set; }
    
    /// <summary>
    /// FK do plano de manutencao.
    /// </summary>
    public long? PlanoManutencaoId { get; set; }
    
    /// <summary>
    /// Tipo de estratégia herdada para este tipo de manutencao.
    /// </summary>
    [ForeignKey(nameof(TipoEstrategiaId))]
    public virtual TipoEstrategiaModel? TipoEstrategia { get; set; }
    
    /// <summary>
    /// Plano de manutencao associado a este tipo de manutencao.
    /// </summary>
    [ForeignKey(nameof(PlanoManutencaoId))]
    public virtual PlanoManutencaoModel? PlanoManutencao { get; set; }

    /// <summary>
    /// Guarda ferramentas e kits específicos dos equipamentos 
    /// </summary>
    public virtual ICollection<FerramentasTipoModel>? Ferramentas { get; set; } = new List<FerramentasTipoModel>();
    
    /// <summary>
    /// Guarda itens de renovação para o cadastro de tipos de ativo
    /// </summary>
    public virtual ICollection<ItemRenovacaoTipoModel>? ItensRenovacao { get; set; } = new List<ItemRenovacaoTipoModel>();

    /// <summary>
    /// Tarefas relacionadas a este tipo de manutencao
    /// </summary>
    public virtual ICollection<TarefaModel>? ListaTarefa { get; set; } = new List<TarefaModel>();
}

/// <summary>
/// Qual o tipo do tipo cadastrado.
/// </summary>
public enum TipoTipo
{
    Manutencao = 0,
    RenovacaoAtivo = 1
}