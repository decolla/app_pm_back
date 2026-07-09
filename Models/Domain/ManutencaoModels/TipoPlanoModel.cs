using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de domínio para os tipos de planos de manutenção.
/// </summary>
public class TipoPlanoModel
{
    /// <summary>
    /// Identificador único do Tipo de Plano.
    /// Exemplo: 1
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdTipoPlano { get; set; }

    /// <summary>
    /// Define se o plano está visível no sistema.
    /// Exemplo: true
    /// </summary>
    public bool? Visivel { get; set; }

    /// <summary>
    /// Quantidade de dias de duração ou ciclo.
    /// Exemplo: 30
    /// </summary>
    public int? Dias { get; set; }

    /// <summary>
    /// Margem de tolerância positiva para execução.
    /// Exemplo: 5.0
    /// </summary>
    public double? ToleranciaPositiva { get; set; }

    /// <summary>
    /// Margem de tolerância negativa para execução.
    /// Exemplo: 2.5
    /// </summary>
    public double? ToleranciaNegativa { get; set; }

    /// <summary>
    /// Indica se a estratégia é contínua.
    /// Exemplo: false
    /// </summary>
    public bool? Continua { get; set; }

    /// <summary>
    /// Categoria descritiva do plano.
    /// Exemplo: "Manutenção ou renovação"
    /// </summary>
    public CategoriaTipoManutencao Categoria { get; set; }

    /// <summary>
    /// FK para o Tipo de Estratégia.
    /// </summary>
    public long? IdTipoEstrategia { get; set; }

    /// <summary>
    /// FK para o Plano.
    /// </summary>
    public long? IdPlano { get; set; }

    /// <summary>
    /// FK para o Tipo de Manutenção.
    /// </summary>
    public long? IdTipoManutencao { get; set; }

    /// <summary>
    /// FK para a Nota associada, se aplicável.
    /// </summary>
    public long? IdNota { get; set; }
    
    /// <summary>
    /// FK para a Ordem associada, se aplicável.
    /// </summary>
    public long? IdOrdem { get; set; }
    
    /// <summary>
    /// FK para o Tipo de CEP associado, se aplicável.
    /// </summary>
    public long? IdTipoCep { get; set; }

    /// <summary>
    /// Objeto de navegação para a Estratégia vinculada.
    /// </summary>
    [ForeignKey(nameof(IdTipoEstrategia))]
    public virtual TipoEstrategiaModel? TipoEstrategia { get; set; }

    /// <summary>
    /// Objeto de navegação para o Plano vinculado.
    /// </summary>
    [ForeignKey(nameof(IdPlano))]
    public virtual PlanoModel? Plano { get; set; }

    /// <summary>
    /// Objeto de navegação para o Tipo de Manutenção vinculado.
    /// </summary>
    [ForeignKey(nameof(IdTipoManutencao))]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }
    
    /// <summary>
    /// Objeto de navegação para a Nota vinculada, se aplicável.
    /// </summary>
    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
    
    /// <summary>
    /// Objeto de navegação para a Ordem vinculada, se aplicável.
    /// </summary>
    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemModel? Ordem { get; set; }
    
    /// <summary>
    /// Objeto de navegação para o Tipo de CEP vinculado, se aplicável.
    /// </summary>
    [ForeignKey(nameof(IdTipoCep))]
    public virtual TipoCepModel? TipoCep { get; set; }
    
}
