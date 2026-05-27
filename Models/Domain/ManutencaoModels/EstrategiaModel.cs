using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa a definição de uma estratégia de manutenção (ex: Preventiva, Preditiva) e suas regras de periodicidade.
/// </summary>
/// <example>
/// <code>
/// var estrategia = new EstrategiaModel { 
///     Nome = "Preventiva Semestral", 
///     TipoAtivoAplicavel = "Motor Elétrico",
///     Habilitada = true 
/// };
/// </code>
/// </example>
[Table("Estrategias")]
public class EstrategiaModel
{
    /// <summary>
    /// Identificador único e chave primária da estratégia.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEstrategia { get; set; }

    /// <summary>
    /// Nome descritivo da estratégia de manutenção.
    /// </summary>
    /// <example>Manutenção Preventiva Nível 1</example>
    [Required(ErrorMessage = "O nome da estratégia é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
    public required string Nome { get; set; }

    /// <summary>
    /// Descrição detalhada ou definição técnica dos ciclos associados (ex: "500h, 1000h").
    /// </summary>
    /// <example>A cada 6 meses ou 1000 horas de operação.</example>
    [StringLength(150, ErrorMessage = "A descrição dos ciclos não pode exceder 150 caracteres.")]
    public string? Ciclos { get; set; }

    /// <summary>
    /// Define se a estratégia está disponível para novos planos de manutenção.
    /// </summary>
    public bool Habilitada { get; set; } = true;

    /// <summary>
    /// Categoria ou classe de ativos aos quais esta estratégia é pertinente.
    /// </summary>
    /// <example>Compressores de Parafuso</example>
    [Required(ErrorMessage = "O tipo de ativo aplicável deve ser informado.")]
    [StringLength(100, ErrorMessage = "O tipo de ativo deve ter no máximo 100 caracteres.")]
    public string? TipoAtivoAplicavel { get; set; }

    /// <summary>
    /// Quantidade total de etapas ou ciclos que compõem esta estratégia antes de uma possível reinicialização.
    /// </summary>
    /// <example>12</example>
    [Range(0, int.MaxValue, ErrorMessage = "O número de ciclos não pode ser negativo.")]
    public int? NumeroCiclos { get; set; }

    /// <summary>
    /// Se verdadeiro, ao finalizar o último ciclo, a estratégia retorna automaticamente ao primeiro ciclo.
    /// </summary>
    public bool? CicloContinuo { get; set; }

    /// <summary>
    /// Chave estrangeira para o Escopo de manutenção relacionado.
    /// </summary>
    public int? IdEscopo { get; set; }

    /// <summary>
    /// Relacionamento com os tipos específicos de estratégia (ex: Mecânica, Elétrica).
    /// </summary>
    public virtual ICollection<TipoEstrategiaModel>? TiposEstrategia { get; set; } = new List<TipoEstrategiaModel>();

    /// <summary>
    /// Lista de planos de manutenção que estão baseados nesta estratégia.
    /// </summary>
    public virtual ICollection<PlanoModel>? Planos { get; set; } = new List<PlanoModel>();
}
