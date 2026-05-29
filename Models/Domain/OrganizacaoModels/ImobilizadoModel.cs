using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa um ativo imobilizado no sistema de patrimônio.
/// </summary>
public class ImobilizadoModel
{
    /// <summary>
    /// Identificador único do imobilizado.
    /// </summary>
    [Key]
    public int IdImobilizado { get; set; }

    /// <summary>
    /// Nome descritivo do item.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Denominação técnica ou contábil.
    /// </summary>
    [MaxLength(150)]
    public required string Denominacao { get; set; }

    /// <summary>
    /// Número de série do fabricante.
    /// </summary>
    [MaxLength(100)]
    public required string Nserie { get; set; }

    /// <summary>
    /// Quantidade física disponível.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Quantidade { get; set; }

    /// <summary>
    /// Data em que o item foi adquirido.
    /// </summary>
    public DateTime? DataAquisicao { get; set; }

    /// <summary>
    /// Data em que o item foi incorporado ao patrimônio.
    /// </summary>
    public DateTime? DataIncorporacao { get; set; }

    /// <summary>
    /// Valor original de compra.
    /// </summary>
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ValorCompra { get; set; }

    /// <summary>
    /// Código da moeda (ex: BRL, USD).
    /// </summary>
    [MaxLength(10)]
    public required string Moeda { get; set; } 

    /// <summary>
    /// Taxa ou valor acumulado de depreciação.
    /// </summary>
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Depreciacao { get; set; }

    /// <summary>
    /// Valor contábil atual após depreciação.
    /// </summary>
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ValorAtual { get; set; }

    /// <summary>
    /// Observações ou informações complementares.
    /// </summary>
    [MaxLength(1000)]
    public string? InfoAdicionais { get; set; }
}
