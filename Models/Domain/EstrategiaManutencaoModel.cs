using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Estrategia de manutencao usada para calcular ciclos e abertura de ordens.
/// </summary>
public class EstrategiaManutencaoModel
{
    /// <summary>
    /// Identificador da estrategia.
    /// </summary>
    [Key]
    public int IdEstrategia { get; set; }

    /// <summary>
    /// Nome da estrategia.
    /// </summary>
    [MaxLength(80)]
    public required string Nome { get; set; }

    /// <summary>
    /// Denominacao da estrategia.
    /// </summary>
    [MaxLength(150)]
    public string? Denominacao { get; set; }

    /// <summary>
    /// Codigo de programacao da estrategia.
    /// </summary>
    [MaxLength(40)]
    public string? CodigoProgramacao { get; set; }

    /// <summary>
    /// Unidade estrategica da estrategia.
    /// </summary>
    [MaxLength(20)]
    public string? UnidadeEstrategica { get; set; }

    /// <summary>
    /// Horizonte de abertura de ordens.
    /// </summary>
    public int? HorizonteAbertura { get; set; }

    /// <summary>
    /// Fator de deslocamento na confirmacao atrasada.
    /// </summary>
    public decimal? FatorDeslocamentoAtrasada { get; set; }

    /// <summary>
    /// Tolerancia percentual para confirmacao atrasada.
    /// </summary>
    public decimal? ToleranciaAtrasadaPercentual { get; set; }

    /// <summary>
    /// Fator de deslocamento na confirmacao antecipada.
    /// </summary>
    public decimal? FatorDeslocamentoAntecipada { get; set; }

    /// <summary>
    /// Tolerancia percentual para confirmacao antecipada.
    /// </summary>
    public decimal? ToleranciaAntecipadaPercentual { get; set; }

    /// <summary>
    /// Calendario de fabrica utilizado.
    /// </summary>
    [MaxLength(30)]
    public string? CalendarioFabrica { get; set; }

    /// <summary>
    /// Planos que usam esta estrategia.
    /// </summary>
    public virtual ICollection<PlanoManutencaoModel> Planos { get; set; } = new List<PlanoManutencaoModel>();
}

