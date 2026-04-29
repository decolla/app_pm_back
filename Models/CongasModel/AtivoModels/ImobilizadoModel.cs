using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Dados do imobilizado associado aos ativos de manutencao.
/// </summary>
public class ImobilizadoModel
{
    /// <summary>
    /// TAG de identificacao do imobilizado.
    /// </summary>
    [Key]
    [MaxLength(60)]
    public required string TagImobilizado { get; set; }

    /// <summary>
    /// Denominacao do imobilizado.
    /// </summary>
    [MaxLength(150)]
    public string? Denominacao { get; set; }

    /// <summary>
    /// Nome do imobilizado.
    /// </summary>
    [MaxLength(150)]
    public string? NomeImobilizado { get; set; }

    /// <summary>
    /// Descricao detalhada.
    /// </summary>
    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Informacao adicional do imobilizado.
    /// </summary>
    [MaxLength(500)]
    public string? InformacaoAdicional { get; set; }

    /// <summary>
    /// Numero de serie.
    /// </summary>
    [MaxLength(80)]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Numero de inventario.
    /// </summary>
    [MaxLength(80)]
    public string? NumeroInventario { get; set; }

    /// <summary>
    /// Quantidade contabilizada.
    /// </summary>
    public decimal? Quantidade { get; set; }

    /// <summary>
    /// Data de aquisicao.
    /// </summary>
    public DateTime? DataAquisicao { get; set; }

    /// <summary>
    /// Data de incorporacao.
    /// </summary>
    public DateTime? DataIncorporacao { get; set; }

    /// <summary>
    /// Valor de compra.
    /// </summary>
    public decimal? ValorCompra { get; set; }

    /// <summary>
    /// Depreciacao anual.
    /// </summary>
    public decimal? DepreciacaoAno { get; set; }

    /// <summary>
    /// Valor atual calculado.
    /// </summary>
    public decimal? ValorAtual { get; set; }

    /// <summary>
    /// Locais de instalacao vinculados ao imobilizado.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();

    /// <summary>
    /// Equipamentos vinculados ao imobilizado.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();
}
