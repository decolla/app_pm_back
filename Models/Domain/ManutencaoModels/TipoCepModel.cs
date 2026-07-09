using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de Tipo de CEP (Controle Estatístico de Processo) para ativos.
/// Exemplo: TipoAtivo = "Motor Elétrico", Confiabilidade = 0.95.
/// </summary>
public class TipoCepModel
{
    /// <summary>
    /// Identificador único do Tipo de CEP.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdTipoCep { get; set; }

    /// <summary>
    /// Descrição do tipo de ativo associado.
    /// Exemplo: "Bomba Hidráulica"
    /// </summary>
    [MaxLength(100)]
    public string? TipoAtivo { get; set; }

    /// <summary>
    /// Índice de confiabilidade esperada para este tipo de ativo.
    /// Exemplo: 0.85
    /// </summary>
    public double? Confiabilidade { get; set; }

    /// <summary>
    /// Define se o registro está ativo no sistema.
    /// Exemplo: true
    /// </summary>
    public bool? Habilitada { get; set; }

    /// <summary>
    /// Campo para informações genéricas ou futuras.
    /// </summary>
    public string? CampoIndefinido { get; set; }

    /// <summary>
    /// Lista de ações a serem tomadas em caso de falha, armazenada em formato JSONB.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<AcoesFalha>? AcoesFalha { get; set; } = new();

    /// <summary>
    /// Coleção de estratégias vinculadas a este tipo de CEP.
    /// </summary>
    public virtual ICollection<TipoEstrategiaModel>? TiposEstrategia { get; set; } = new List<TipoEstrategiaModel>();
}
