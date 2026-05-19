using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Atividade sugerida para execucao da nota.
/// </summary>
public class AtividadeNotaModel
{
    /// <summary>
    /// Identificador da atividade.
    /// </summary>
    [Key]
    public int IdAtividade { get; set; }

    /// <summary>
    /// Nota associada a atividade.
    /// </summary>
    public long NotaId { get; set; }

    /// <summary>
    /// Descricao da atividade.
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

