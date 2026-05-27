using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa a configuração de uma estratégia de manutenção específica.
/// Exemplo: Uma estratégia de 30 dias com 5% de tolerância.
/// </summary>
public class TipoEstrategiaModel
{
    /// <summary>
    /// Identificador único do Tipo de Estratégia.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdTipoEstrategia { get; set; }

    /// <summary>
    /// Quantidade de dias para o ciclo. Exemplo: 90
    /// </summary>
    public int? Dias { get; set; }

    /// <summary>
    /// Margem de atraso permitida. Exemplo: 0.10 (10%)
    /// </summary>
    public double? ToleranciaPositiva { get; set; }

    /// <summary>
    /// Margem de antecipação permitida. Exemplo: 0.05 (5%)
    /// </summary>
    public double? ToleranciaNegativa { get; set; }

    /// <summary>
    /// Define se a estratégia é recorrente. Exemplo: true
    /// </summary>
    public bool? Continua { get; set; }

    /// <summary>
    /// Categoria da estratégia. Exemplo: "Preventiva"
    /// </summary>
    [MaxLength(100)]
    public string? Categoria { get; set; }

    /// <summary>
    /// Chave estrangeira para a Estratégia pai.
    /// </summary>
    public int? IdEstrategia { get; set; }

    /// <summary>
    /// Chave estrangeira para o Tipo de Manutenção.
    /// </summary>
    public int? IdTipoManutencao { get; set; }

    /// <summary>
    /// Chave estrangeira para o Tipo de CEP.
    /// </summary>
    public int? IdTipoCep { get; set; }

    /// <summary>
    /// Objeto de navegação da Estratégia.
    /// </summary>
    [ForeignKey(nameof(IdEstrategia))]
    public virtual EstrategiaModel? Estrategia { get; set; }

    /// <summary>
    /// Objeto de navegação do Tipo de Manutenção.
    /// </summary>
    [ForeignKey(nameof(IdTipoManutencao))]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }

    /// <summary>
    /// Objeto de navegação do Tipo de CEP.
    /// </summary>
    [ForeignKey(nameof(IdTipoCep))]
    public virtual TipoCepModel? TipoCep { get; set; }
    
    /// <summary>
    /// Coleção de planos vinculados a este tipo de CEP.
    /// </summary>
    public virtual ICollection<TipoPlanoModel>? TiposPlano { get; set; } = new List<TipoPlanoModel>();
}
