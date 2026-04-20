using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using app_pm_back.Models.Domain;

namespace app_pm_back.Models.CongasModel;

public class TipoEstrategiaModel
{
    /// <summary>
    /// Id do item da estratégia.
    /// </summary>
    [Key]
    public int IdTipoEstrategia { get; set; }
    
    /// <summary>
    /// Nome do tipo da estratégia 
    /// </summary>
    [MaxLength(80)]
    public string NomeTipo { get; set; }
    
    /// <summary>
    /// Descrição para o tipo 
    /// </summary>
    [MaxLength(500)]
    public string Descricao { get; set; }

    /// <summary>
    /// Número de meses que ocorrerá a manutenção do tipo especificado
    /// </summary>
    public int Meses { get; set; } = 80;
    
    /// <summary>
    /// Tolerância positiva para a manutenção do tipo especificado
    /// </summary>
    public int ToleranciaPositiva { get; set; }

    /// <summary>
    /// Unidade de medida para a tolerancia positiva
    /// </summary>
    [MaxLength(80)]
    public string UnidadeToleranciaPositiva { get; set; }

    /// <summary>
    /// Tolerância negativa para a manutenção do tipo especificado
    /// </summary>
    public int ToleranciaNegativa { get; set; }
    
    /// <summary>
    /// Unidade de medida para a tolerancia negativa
    /// </summary>
    [MaxLength(80)]
    public string UnidadeToleranciaNegativa { get; set; }
    
    /// <summary>
    /// FK da estrategia associada.
    /// </summary>
    public int? EstrategiaId { get; set; }
    
    /// <summary>
    /// Estrategia associada ao plano.
    /// </summary>
    [ForeignKey(nameof(EstrategiaId))]
    [JsonIgnore]
    public virtual EstrategiaManutencaoModel? Estrategia{ get; set; }
    
    /// <summary>
    /// Tipos de manutenção que herdam deste tipo de estratégia.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<TipoManutencaoModel> TiposManutencao { get; set; } = new List<TipoManutencaoModel>();
}