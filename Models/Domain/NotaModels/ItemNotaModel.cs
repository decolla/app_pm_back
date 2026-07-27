using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa um item individual de uma nota técnica ou de manutenção.
/// Armazena detalhes sobre falhas, diagnósticos e a relação com a nota principal.
/// </summary>
/// <example>
/// <code>
/// var item = new ItemNotaModel { 
///     Componente = "Motor Elétrico", 
///     Sintoma = "Superaquecimento", 
///     Causa = "Falta de lubrificação" 
/// };
/// </code>
/// </example>
public class ItemNotaModel : IAuditable
{
    /// <summary>
    /// Identificador único do item da nota (Chave Primária).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdItemNota { get; set; }

    /// <summary>
    /// Nome ou descrição do componente afetado.
    /// </summary>
    /// <value>Máximo de 150 caracteres.</value>
    [MaxLength(150)]
    [Display(Name = "Componente")]
    public string? Componente { get; set; }

    /// <summary>
    /// Descrição do sintoma observado no componente.
    /// </summary>
    /// <value>Máximo de 150 caracteres.</value>
    [MaxLength(150)]
    [Display(Name = "Sintoma")]
    public string? Sintoma { get; set; }

    /// <summary>
    /// Descrição da causa provável ou confirmada do problema.
    /// </summary>
    /// <value>Máximo de 150 caracteres.</value>
    [MaxLength(150)]
    [Display(Name = "Causa")]
    public string? Causa { get; set; }

    /// <summary>
    /// Chave estrangeira para a nota pai (<see cref="NotaModel"/>).
    /// </summary>
    [Required(ErrorMessage = "O vínculo com uma Nota é obrigatório.")]
    public long IdNota { get; set; }

    /// <summary>
    /// Propriedade de navegação para a entidade Nota relacionada.
    /// </summary>
    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
}
