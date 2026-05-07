using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
    /// Nome da estratégia.
    /// </summary>
    [MaxLength(80)]
    public required string Nome { get; set; }

    /// <summary>
    /// Duração de meses do ciclo de manutencao.
    /// </summary>
    public int Ciclos { get; set; }
    
    /// <summary>
    /// Unidade de tempo do ciclo de manutencao.
    /// </summary>
    /// <example> Ano = 0, Mes = 1, Dia = 2</example>
    public UnidadeCiclo UnidadeCiclo { get; set; }

    /// <summary>
    /// Se a estratégia está habilitada para uso ou não.
    /// </summary>
    public bool Habilitada { get; set; } = true;

    /// <summary>
    /// Denominacao da estratégia.
    /// </summary>
    [MaxLength(150)]
    public string? Denominacao { get; set; }

    /// <summary>
    /// Quantidade de ciclos de manutencao previstos para o plano.
    /// </summary>
    public int NumeroCiclos { get; set; }

    /// <summary>
    /// Indica se o ciclo se aplica continuamente.
    /// </summary>
    public bool CicloContinuo { get; set; }

    /// <summary>
    /// Tipo de ativo aplicável 
    /// </summary>
    /// <example>Nível A</example>
    public string TipoAtivoAplicavel { get; set; }
    
    /// <summary>
    /// Planos que usam esta estrategia.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<PlanoManutencaoModel> Planos { get; set; } = new List<PlanoManutencaoModel>();
    
    /// <summary>
    /// Tipos associados a esta estrategia.
    /// </summary>
    [JsonIgnore]
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

public enum UnidadeCiclo
{
    Ano = 0,
    Mes = 1,
    Dia = 2
}
