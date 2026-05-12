using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using app_pm_back.Models.Domain;
using app_pm_back.Models.Domain.PlanoManutencaoModels;

namespace app_pm_back.Models.CongasModel;

public class TipoPlanoModel
{
    /// <summary>
    /// Id do tipo de manutencao.
    /// </summary>
    [Key]
    public int IdTipoManutencao { get; set; } 

    /// <summary>
    /// Se o tipo está visível.
    /// </summary>
    /// <example>Invisivel = 0, Visivel = 1, Nova = 2</example>
    public Visivel Visivel { get; set; } = Visivel.Visivel;
    
    /// <summary>
    /// Categoria do tipo de manutencao.
    /// </summary>
    /// <example>Manutencao = 0, Renovacao = 1, CEP = 2</example>
    public CategoriaTipoEstrategia Categoria { get; set; }

    /// <summary>
    /// Número de meses que ocorrerá a manutenção do tipo especificado
    /// </summary>
    public int Dias { get; set; } = 80;
    
    /// <summary>
    /// Tolerância em dias positiva para a manutenção do tipo especificado
    /// </summary>
    public int ToleranciaPositiva { get; set; }

    /// <summary>
    /// Tolerância em dias negativa para a manutenção do tipo especificado
    /// </summary>
    public int ToleranciaNegativa { get; set; }

    /// <summary>
    /// Se o tipo de estratégia é contínua ou não.
    /// </summary>
    public bool Continua { get; set; } = false;

    /// <summary>
    /// FK do tipo de estratégia.
    /// </summary>
    public int? TipoEstrategiaId { get; set; }

    /// <summary>
    /// Tipo de estratégia herdada para este tipo de manutencao.
    /// </summary>
    [ForeignKey(nameof(TipoEstrategiaId))]
    [JsonIgnore]
    public virtual TipoEstrategiaModel? TipoEstrategia { get; set; }

    /// <summary>
    /// FK do plano de manutencao.
    /// </summary>
    public long? PlanoManutencaoId { get; set; }

    /// <summary>
    /// Plano de manutencao associado a este tipo de manutencao.
    /// </summary>
    [ForeignKey(nameof(PlanoManutencaoId))]
    [JsonIgnore]
    public virtual PlanoManutencaoModel? PlanoManutencao { get; set; }
    
    /// <summary>
    /// FK do tipo de manutencao CEP.
    /// </summary>
    public int? TipoManutencaoCepId { get; set; }
    
    /// <summary>
    /// Tipo de manutencao CEP associado a este tipo de manutencao.
    /// </summary>
    [ForeignKey(nameof(TipoManutencaoCepId))]
    [JsonIgnore]
    public virtual TipoManutencaoCepModel? TipoManutencaoCep { get; set; }
    
    /// <summary>
    /// FK do tipo de manutencao.
    /// </summary>
    public int? TipoManutencaoId { get; set; }

    /// <summary>
    /// Tipo de manutencao associado a este tipo de manutencao.
    /// </summary>
    [ForeignKey(nameof(TipoManutencaoId))]
    [JsonIgnore]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }

    /// <summary>
    /// Tarefas relacionadas a este tipo de manutencao
    /// </summary>
    public virtual ICollection<TarefaModel>? ListaTarefa { get; set; } = new List<TarefaModel>();

    /// <summary>
    /// Ferramentas relacionadas a este tipo de manutencao.
    /// </summary>
    public virtual ICollection<FerramentasTipoModel>? Ferramentas { get; set; } = new List<FerramentasTipoModel>();
}

public enum Visivel
{
    Invisivel = 0,
    Visivel = 1,
    Nova = 2
}

