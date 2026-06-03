using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de domínio para um Plano de Manutenção.
/// </summary>
public class PlanoModel
{
    /// <summary>
    /// Identificador único do Plano.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdPlano { get; set; }

    /// <summary>
    /// Nome descritivo do plano. Campo obrigatório.
    /// </summary>
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O Nome não pode exceder 150 caracteres.")]
    public required string Nome { get; set; }

    /// <summary>
    /// Nível de prioridade associado ao plano.
    /// </summary>
    public Prioridade? Prioridade { get; set; }

    /// <summary>
    /// Data prevista para o início das atividades do plano.
    /// </summary>
    public DateTime? DataInicios { get; set; }

    /// <summary>
    /// Define se o plano deve gerar ordens ou ações automaticamente.
    /// </summary>
    public GerarAutomaticamente? GerarAutomaticamente { get; set; }

    /// <summary>
    /// Tempo de antecedência (em dias ou unidade definida) para geração/aviso.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "A antecedência deve ser um valor positivo.")]
    public int? Antecedencia { get; set; }

    /// <summary>
    /// Data em que o plano foi ou deve ser concluído.
    /// </summary>
    public DateTime? DataConclusao { get; set; }

    /// <summary>
    /// Identificador do Ativo vinculado.
    /// </summary>
    public ComponentType? IdAtivo { get; set; }
    
    /// <summary>
    /// Chave estrangeira para a localização de instalação do ativo.
    /// Permite vincular a um local específico de instalação.
    /// </summary>
    [MaxLength(100)]
    public string? IdLi { get; set; }

    /// <summary>
    /// Chave estrangeira para a Estratégia de manutenção.
    /// </summary>
    public int? IdEstrategia { get; set; }

    /// <summary>
    /// Propriedade de navegação para a Estratégia.
    /// </summary>
    [ForeignKey(nameof(IdEstrategia))]
    public virtual EstrategiaModel? Estrategia { get; set; }

    /// <summary>
    /// Coleção de tipos associados a este plano.
    /// </summary>
    public virtual ICollection<TipoPlanoModel>? TiposPlano { get; set; } = new List<TipoPlanoModel>();

    /// <summary>
    /// Coleção de notas ou observações vinculadas ao plano.
    /// </summary>
    public virtual ICollection<NotaModel>? Notas { get; set; } = new List<NotaModel>();
}
