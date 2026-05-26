using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class AnexoNotaModel
{
    [Key]
    public int IdAnexoNota { get; set; }

    [MaxLength(250)]
    public required string Nome { get; set; }

    [MaxLength(100)]
    public required string TipoArquivo { get; set; } // acts as MIME type

    public byte[]? Dados { get; set; } // Binary data of the file

    public long IdNota { get; set; }

    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
}
