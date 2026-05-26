using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class VariavelNotaModel
{
    [Key]
    public int IdVariavel { get; set; }

    public double ValorNumerico { get; set; }

    [MaxLength(20)]
    public string? UnidadeMedida { get; set; }

    [MaxLength(500)]
    public string? Comentarios { get; set; }

    public long IdNota { get; set; }

    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
}
