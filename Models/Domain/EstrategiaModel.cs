using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

public class EstrategiaModel
{
    [Key]
    public int IdEstrategia { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    [MaxLength(150)]
    public string? Ciclos { get; set; }

    public bool Habilitada { get; set; }

    [MaxLength(100)]
    public required string TipoAtivoAplicavel { get; set; }

    public int NumeroCiclos { get; set; }

    public bool CicloContinuo { get; set; }

    public int IdEscopo { get; set; } // Stores Escopo ID only
}
