using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Variavel registrada na nota.
/// </summary>
public class VariavelNotaModel
{
    /// <summary>
    /// Identificador da variavel.
    /// </summary>
    [Key]
    public int IdVariavel { get; set; }

    /// <summary>
    /// Nota associada.
    /// </summary>
    public long NotaId { get; set; }

    /// <summary>
    /// Nome da variavel.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Valor numerico da variavel.
    /// </summary>
    public decimal? ValorNumerico { get; set; }

    /// <summary>
    /// Unidade de medida informada.
    /// </summary>
    [MaxLength(30)]
    public string? UnidadeMedida { get; set; }

    /// <summary>
    /// Comentarios adicionais.
    /// </summary>
    [MaxLength(500)]
    public string? Comentarios { get; set; }

    /// <summary>
    /// Nota associada.
    /// </summary>
    [ForeignKey(nameof(NotaId))]
    [JsonIgnore]
    public virtual NotaManutencaoModel Nota { get; set; } = null!;
}

