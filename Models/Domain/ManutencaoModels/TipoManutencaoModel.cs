using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de domínio para os tipos de manutenção do sistema.
/// </summary>
public class TipoManutencaoModel
{
    /// <summary>
    /// Identificador único do Tipo de Manutenção.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdTipoManutencao { get; set; }
    
    /// <summary>
    /// Cor da bolinha
    /// </summary>
    public string Cor { get; set; } = "#FF0000";
    
    /// <summary>
    /// Categoria associada ao tipo de manutenção.
    /// </summary>
    [MaxLength(100)]
    public CategoriaTipoManutencao Categoria { get; set; }

    /// <summary>
    /// Nome descritivo do tipo de manutenção. Campo obrigatório.
    /// </summary>
    [MaxLength(150)]
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public required string Nome { get; set; }

    /// <summary>
    /// Define a qual tipo de ativo esta manutenção se aplica. Campo obrigatório.
    /// </summary>
    public TiposLI TipoLI { get; set; }
    
    /// <summary>
    /// Define a qual tipo de ativo esta manutenção se aplica
    /// </summary>
    public ComponentType? TipoAtivo { get; set; }

    /// <summary>
    /// Índice de confiabilidade esperado.
    /// </summary>
    public double? Confiabilidade { get; set; }

    /// <summary>
    /// Indica se o registro está ativo para uso no sistema.
    /// </summary>
    public bool? Habilitada { get; set; }

    /// <summary>
    /// Relacionamentos e coleções associadas.
    /// </summary>
    public virtual ICollection<TipoEstrategiaModel>? TiposEstrategia { get; set; } = new List<TipoEstrategiaModel>();
    
    public virtual ICollection<OperacaoModel>? Operacoes { get; set; } = new List<OperacaoModel>();
    
    public virtual ICollection<TipoPlanoModel>? TiposPlano { get; set; } = new List<TipoPlanoModel>();
}
