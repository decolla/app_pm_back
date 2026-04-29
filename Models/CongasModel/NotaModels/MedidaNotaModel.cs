using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Medida sugerida para tratamento da nota.
/// </summary>
public class MedidaNotaModel
{
    /// <summary>
    /// Identificador da medida.
    /// </summary>
    [Key]
    public int IdMedida { get; set; }

    /// <summary>
    /// Nota associada a medida.
    /// </summary>
    public long NotaId { get; set; }

    /// <summary>
    /// Texto da medida sugerida.
    /// </summary>
    [MaxLength(500)]
    public required string Descricao { get; set; }

    /// <summary>
    /// Nota associada.
    /// </summary>
    [ForeignKey(nameof(NotaId))]
    [JsonIgnore]
    public virtual NotaManutencaoModel Nota { get; set; } = null!;
}

