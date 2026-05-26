using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class AnexoOrdemModel
{
    [Key]
    public int IdAnexoOrdem { get; set; }

    [MaxLength(250)]
    public required string Nome { get; set; }

    [MaxLength(100)]
    public required string TipoArquivo { get; set; } // acts as MIME type

    [MaxLength(100)]
    public string? TipoAnexo { get; set; }

    public byte[]? Dados { get; set; } // Binary data of the file

    public long IdOrdem { get; set; }

    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemModel? Ordem { get; set; }
}
