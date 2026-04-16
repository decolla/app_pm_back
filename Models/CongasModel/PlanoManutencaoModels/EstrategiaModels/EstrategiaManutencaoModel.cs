using System.ComponentModel.DataAnnotations;
using app_pm_back.Models.CongasModel;

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
    /// Se a estratégia está habilitada para uso ou não.
    /// </summary>
    public bool Habilitada { get; set; } = true;

    /// <summary>
    /// Duração de meses do ciclo de manutencao.
    /// </summary>
    public int Ciclo { get; set; }
    
    /// <summary>
    /// Quantidade de ciclos de manutencao previstos para o plano.
    /// </summary>
    public int NumeroCiclos { get; set; }
    
    /// <summary>
    /// Tipo de ativo aplicável 
    /// </summary>
    /// <example>Nível A</example>
    public string TipoAtivoAplicável { get; set; }

    /// <summary>
    /// Planos que usam esta estrategia.
    /// </summary>
    public virtual ICollection<PlanoManutencaoModel> Planos { get; set; } = new List<PlanoManutencaoModel>();
    
    public virtual ICollection<TipoEstrategiaModel> Itens { get; set; } = new List<TipoEstrategiaModel>();
    
    
    // /// <summary>
    // /// Horizonte de abertura de ordens.
    // /// </summary>
    // public int? HorizonteAbertura { get; set; }
    //
    // /// <summary>
    // /// Fator de deslocamento na confirmacao atrasada.
    // /// </summary>
    // public decimal? FatorDeslocamentoAtrasada { get; set; }
    //
    // /// <summary>
    // /// Tolerancia percentual para confirmacao atrasada.
    // /// </summary>
    // public decimal? ToleranciaAtrasadaPercentual { get; set; }
    //
    // /// <summary>
    // /// Fator de deslocamento na confirmacao antecipada.
    // /// </summary>
    // public decimal? FatorDeslocamentoAntecipada { get; set; }
    //
    // /// <summary>
    // /// Tolerancia percentual para confirmacao antecipada.
    // /// </summary>
    // public decimal? ToleranciaAntecipadaPercentual { get; set; }
    //
    // /// <summary>
    // /// Calendario de fabrica utilizado.
    // /// </summary>
    // [MaxLength(30)]
    // public string? CalendarioFabrica { get; set; }
}

