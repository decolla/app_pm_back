using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

/// <summary>
/// Estrategia de manutencao aplicada aos planos COMGAS.
/// </summary>
public class EstrategiaComgasModel
{
    /// <summary>
    /// Id da estratégia
    /// </summary>
    [Key]
    public int IdEstrategia { get; set; }
    
    /// <summary>
    /// Nome da estratégia 
    /// </summary>
    [MaxLength(80)]
    [SheetColumn("Estratégia")]
    public string? NomeEstrategia { get; set; }
    
    /// <summary>
    /// Denominação da estratégia 
    /// </summary>
    [MaxLength(150)]
    public string? Denominacao { get; set; }
    
    /// <summary>
    /// Unidade estratégica da estratégia
    /// </summary>
    [MaxLength(20)]
    public string? UnidadeEstrategica { get; set; }
    
    /// <summary>
    /// Horizonte de abertura da estratégia, ou seja, quantos ciclos futuros a estratégia deve considerar para abertura de ordens de manutenção
    /// </summary>
    public int? HorizonteAbertura { get; set; }
    
    /// <summary>
    /// Fator de deslocamento para atraso, ou seja, quantos ciclos de atraso a estratégia deve considerar para abertura de ordens de manutenção
    /// </summary>
    public decimal? FatorDeslocamentoAtraso { get; set; }
    public decimal? ToleranciaAtraso { get; set; }
    public decimal? FatorDeslocamentoAntecipado { get; set; }
    public decimal? ToleranciaAntecipada { get; set; }
    
    public virtual ICollection<MaintancePlanComgasModel> Planos { get; set; } = new List<MaintancePlanComgasModel>();
    public virtual ICollection<CicloComgasModel> Ciclos { get; set; } = new List<CicloComgasModel>();
}