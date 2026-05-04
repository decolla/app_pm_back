using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Cadastro de analise de risco vinculada a uma ordem.
/// </summary>
public class AnaliseRiscoModel
{
    /// <summary>
    /// Identificador interno da analise.
    /// </summary>
    [Key]
    public int IdAnaliseRisco { get; set; }

    /// <summary>
    /// Codigo da analise de risco (AR).
    /// </summary>
    [MaxLength(40)]
    public string? CodigoAnaliseRisco { get; set; }

    /// <summary>
    /// Data de criacao da analise.
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Resultado da analise.
    /// </summary>
    [MaxLength(120)]
    public string? ResultadoAnalise { get; set; }

    /// <summary>
    /// TAG exibida para referencia da ordem.
    /// </summary>
    [MaxLength(60)]
    public string? TagOrdem { get; set; }

    /// <summary>
    /// Ordem relacionada a analise.
    /// </summary>
    public long? OrdemId { get; set; }

    /// <summary>
    /// Ordem relacionada.
    /// </summary>
    [ForeignKey(nameof(OrdemId))]
    public virtual OrdemManutencaoModel? Ordem { get; set; }

    /// <summary>
    /// PM_Anexos associados a analise.
    /// </summary>
    public virtual ICollection<AnnexModel> Anexos { get; set; } = new List<AnnexModel>();
}

