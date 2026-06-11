using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa uma ferramenta no sistema.
/// </summary>
public class FerramentaModel
{
    /// <summary>
    /// Identificador único da ferramenta.
    /// </summary>
    [Key]
    public int IdFerramenta { get; set; }

    /// <summary>
    /// Categoria da ferramenta (ex: Elétrica, Manual).
    /// </summary>
    [MaxLength(100)]
    public required string Categoria { get; set; }

    /// <summary>
    /// Nome descritivo da ferramenta.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Quantidade disponível em estoque.
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Tipo específico da ferramenta.
    /// </summary>
    [MaxLength(100)]
    public required string TipoFerramenta { get; set; }

    /// <summary>
    /// Peso bruto da ferramenta.
    /// </summary>
    public double PesoBruto { get; set; }

    /// <summary>
    /// Peso líquido da ferramenta.
    /// </summary>
    public double PesoLiquido { get; set; }

    /// <summary>
    /// Unidade de medida do peso bruto (ex: kg, g).
    /// </summary>
    [MaxLength(20)]
    public string? UmPesoBruto { get; set; }

    /// <summary>
    /// Unidade de medida do peso líquido (ex: kg, g).
    /// </summary>
    [MaxLength(20)]
    public string? UmPesoLiquido { get; set; }

    /// <summary>
    /// Volume ocupado pela ferramenta.
    /// </summary>
    public double Volume { get; set; }

    /// <summary>
    /// Unidade de medida do volume (ex: m³, cm³).
    /// </summary>
    [MaxLength(20)]
    public string? UmVolume { get; set; }

    /// <summary>
    /// Dimensões da ferramenta (ex: 10x20x30).
    /// </summary>
    [MaxLength(50)]
    public string? Dimensao { get; set; }

    /// <summary>
    /// Unidade de medida das dimensões (ex: mm, cm).
    /// </summary>
    [MaxLength(20)]
    public string? UmDimensao { get; set; }

    /// <summary>
    /// Fabricante da ferramenta.
    /// </summary>
    [MaxLength(150)]
    public string? Fabricante { get; set; }

    /// <summary>
    /// Valor de custo na aquisição.
    /// </summary>
    public decimal ValorCompra { get; set; }
    
    /// <summary>
    /// Moeda do valor de compra (ex: BRL, USD).
    /// </summary>
    public string? Moeda { get; set; }

    /// <summary>
    /// Data em que a ferramenta foi adquirida.
    /// </summary>
    public DateTime DataAquisicao { get; set; }

    /// <summary>
    /// Descrição detalhada ou observações sobre a ferramenta.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Identificador do Local de Instalação (LI).
    /// </summary>
    public string IdLI { get; set; } = string.Empty;

    /// <summary>
    /// Identificador do Escopo associado.
    /// </summary>
    public int IdEscopo { get; set; }

    /// <summary>
    /// Identificador do Almoxarifado onde a ferramenta está alocada.
    /// </summary>
    public int IdAlmoxarifado { get; set; }

    /// <summary>
    /// Propriedade de navegação para o Almoxarifado.
    /// </summary>
    [ForeignKey(nameof(IdAlmoxarifado))]
    public virtual AlmoxarifadoModel? Almoxarifado { get; set; }

    /// <summary>
    /// Lista de operações associadas a esta ferramenta.
    /// </summary>
    public virtual ICollection<OperacaoModel> Operacoes { get; set; } = new List<OperacaoModel>();
}
