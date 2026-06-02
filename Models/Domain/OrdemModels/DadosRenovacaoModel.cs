using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa os dados de renovação de um ativo no sistema.
/// Esta entidade armazena o histórico e métricas de confiabilidade resultantes de processos de manutenção.
/// </summary>
/// <example>
/// <code>
/// var renovacao = new DadosRenovacaoModel { 
///     UltimaRenovacao = DateTime.Now, 
///     OrigemUR = "Sistema", 
///     Confiabilidade = 0.95 
/// };
/// </code>
/// </example>
[Table("DadosRenovacao")]
public class DadosRenovacaoModel
{
    /// <summary>
    /// Identificador único do registro de renovação.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdDados { get; set; }

    /// <summary>
    /// Data e hora da última renovação realizada.
    /// </summary>
    [Required(ErrorMessage = "A data da última renovação é obrigatória.")]
    public DateTime UltimaRenovacao { get; set; }

    /// <summary>
    /// Origem da última renovação (ex: sistema, manual, integração).
    /// </summary>
    [MaxLength(100)]
    public OrigemUR OrigemUR { get; set; }

    /// <summary>
    /// Índice de confiabilidade calculado para o ativo (0 a 1 ou percentual).
    /// </summary>
    public double? Confiabilidade { get; set; }

    /// <summary>
    /// Identificador do Ativo relacionado.
    /// </summary>
    public ComponentType? IdAtivo { get; set; }
    
    /// <summary>
    /// Chave estrangeira para a localização de instalação do ativo.
    /// Permite vincular a nota a um local específico de instalação.
    /// </summary>
    [MaxLength(100)]
    public string? IdLi { get; set; }

    /// <summary>
    /// Identificador da Nota relacionada.
    /// </summary>
    public long? IdNota { get; set; }

    /// <summary>
    /// Identificador da Ordem de Serviço relacionada.
    /// </summary>
    public long? IdOrdem { get; set; }

    /// <summary>
    /// Identificador do Usuário que processou a renovação.
    /// </summary>
    public int? IdUsuario { get; set; }
    
    /// <summary>
    /// Referência à Nota relacionada.
    /// </summary>
    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }

    /// <summary>
    /// Referência à Ordem de Serviço relacionada.
    /// </summary>
    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemModel? Ordem { get; set; }

    /// <summary>
    /// Referência ao Usuário que realizou a renovação.
    /// </summary>
    [ForeignKey(nameof(IdUsuario))]
    public virtual UsuarioModel? Usuario { get; set; }
}
