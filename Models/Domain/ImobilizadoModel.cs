using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

public class ImobilizadoModel
{
    [Key]
    public int IdImobilizado { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    [MaxLength(150)]
    public required string Denominacao { get; set; }

    [MaxLength(100)]
    public required string Nserie { get; set; }

    public int Quantidade { get; set; }

    public DateTime DataAquisicao { get; set; }

    public DateTime DataIncorporacao { get; set; }

    public decimal ValorCompra { get; set; }

    [MaxLength(10)]
    public required string Moeda { get; set; }

    public double Depreciacao { get; set; }

    public decimal ValorAtual { get; set; }

    [MaxLength(1000)]
    public string? InfoAdicionais { get; set; }
}
