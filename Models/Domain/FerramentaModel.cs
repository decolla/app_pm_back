using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class FerramentaModel
{
    [Key]
    public int IdFerramenta { get; set; }

    [MaxLength(100)]
    public required string Categoria { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    public int Quantidade { get; set; }

    [MaxLength(100)]
    public required string TipoFerramenta { get; set; }

    public double PesoBruto { get; set; }

    public double PesoLiquido { get; set; }

    [MaxLength(20)]
    public string? UmPesoBruto { get; set; }

    [MaxLength(20)]
    public string? UmPesoLiquido { get; set; }

    public double Volume { get; set; }

    [MaxLength(20)]
    public string? UmVolume { get; set; }

    [MaxLength(50)]
    public string? Dimensao { get; set; }

    [MaxLength(20)]
    public string? UmDimensao { get; set; }

    [MaxLength(150)]
    public string? Fabricante { get; set; }

    public decimal ValorCompra { get; set; }

    public DateTime DataAquisicao { get; set; }

    [MaxLength(300)]
    public string? Descricao { get; set; }

    public int IdLI { get; set; } // Stores Local de Instalação (LI) ID only

    public int IdEscopo { get; set; } // Stores Escopo ID only

    public int IdAlmoxarifado { get; set; }

    [ForeignKey(nameof(IdAlmoxarifado))]
    public virtual AlmoxarifadoModel? Almoxarifado { get; set; }

    public virtual ICollection<OperacaoModel> Operacoes { get; set; } = new List<OperacaoModel>();
}
