using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using app_pm_back.Models.Domain;
using app_pm_back.Models.Domain.PlanoManutencaoModels;

namespace app_pm_back.Models.CongasModel;

public class TipoEstrategiaModel
{
    /// <summary>
    /// Id do item da estratégia.
    /// </summary>
    [Key]
    public int IdTipoEstrategia { get; set; }

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
    /// Categoria atribuida ao tipo de estratégia, indicando a qual tipo de plano de manutenção ela pertence.
    /// </summary>
    /// <example>Manutencao = 0, Renovacao = 1, CEP = 2</example>
    public CategoriaTipoEstrategia Categoria { get; set; }

    /// <summary>
    /// FK da estrategia associada.
    /// </summary>
    public int EstrategiaId { get; set; }

    /// <summary>
    /// Estrategia associada ao plano.
    /// </summary>
    [ForeignKey(nameof(EstrategiaId))]
    [JsonIgnore]
    public virtual EstrategiaManutencaoModel Estrategia { get; set; }

    /// <summary>
    /// FK do tipo CEP associada.
    /// </summary>
    public int? IdTipoManutencaoCep { get; set; }

    [ForeignKey(nameof(IdTipoManutencaoCep))]
    [JsonIgnore]
    public virtual TipoManutencaoCepModel? TipoManutencaoCep {get; set;}

    /// <summary>
    /// FK do tipo de manutenção associada.
    /// </summary>
    public int? IdTipoManutencao { get; set; }

    [ForeignKey(nameof(IdTipoManutencao))]
    [JsonIgnore]
    public virtual TipoManutencaoModel? TipoManutencao {get; set;}

    /// <summary>
    /// Tipos de plano que herdam deste tipo de estratégia.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<TipoPlanoModel> TiposPlanos { get; set; } = new List<TipoPlanoModel>();
}

public enum CategoriaTipoEstrategia
{
    Manutencao = 0,
    Renovacao = 1,
    CEP = 2
}