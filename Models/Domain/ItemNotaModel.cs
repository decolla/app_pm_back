using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class ItemNotaModel
{
    [Key]
    public int IdItemNota { get; set; }

    [MaxLength(150)]
    public string? Componente { get; set; }

    [MaxLength(150)]
    public string? Sintoma { get; set; }

    [MaxLength(150)]
    public string? Causa { get; set; }

    public long IdNota { get; set; }

    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
}
