using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa uma variável associada a uma nota técnica.
/// Exemplo: ValorNumerico = 15.5, UnidadeMedida = "kg".
/// </summary>
public class VariavelNotaModel
{
    /// <summary>
    /// Identificador único da variável.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdVariavel { get; set; }
    
    /// <summary>
    /// Nome da variável registrada. Exemplo: "Temperatura", "Pressão", "Vibração".
    /// </summary>
    [MaxLength(200, ErrorMessage = "O nome da variável não pode exceder 200 caracteres.")]
    public string NomeVariavel { get; set; } = string.Empty;

    /// <summary>
    /// Valor numérico registrado. Exemplo: 10.5
    /// </summary>
    public double? ValorNumerico { get; set; }

    /// <summary>
    /// Unidade de medida do valor. Exemplo: "m", "kg", "°C".
    /// </summary>
    [MaxLength(20)]
    public string? UnidadeMedida { get; set; }

    /// <summary>
    /// Observações adicionais sobre a variável.
    /// </summary>
    [MaxLength(500)]
    public string? Comentarios { get; set; }

    /// <summary>
    /// Chave estrangeira para a nota relacionada.
    /// </summary>
    public long? IdNota { get; set; }

    /// <summary>
    /// Referência de navegação para o objeto Nota.
    /// </summary>
    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
}
