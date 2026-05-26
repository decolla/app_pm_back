using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

public class CpmModel
{
    [Key]
    public int IdCPM { get; set; }

    [MaxLength(150)]
    public required string NomeCentro { get; set; }

    [MaxLength(100)]
    public required string Organizacao { get; set; }

    [MaxLength(300)]
    public string? Descricao { get; set; }

    public int IdEscopo { get; set; } // Stores Escopo ID only
}
