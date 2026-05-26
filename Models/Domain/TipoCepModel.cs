using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class TipoCepModel
{
    [Key]
    public int IdTipoCep { get; set; }

    [MaxLength(100)]
    public required string TipoAtivo { get; set; }

    public double Confiabilidade { get; set; }

    public bool Habilitada { get; set; }

    public string? CampoIndefinido { get; set; }

    [Column(TypeName = "jsonb")]
    public List<AcoesFalha>? AcoesFalha { get; set; } = new();
}
