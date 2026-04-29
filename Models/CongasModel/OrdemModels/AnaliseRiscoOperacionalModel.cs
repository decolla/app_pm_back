using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models;
using app_pm_back.Models.Domain;

/// <summary>
/// Cadastro de analise de risco operacional (ARO) vinculada a uma ordem.
/// </summary>
public class AnaliseRiscoOperacionalModel
{
    /// <summary>
    /// Identificador interno da analise.
    /// </summary>
    [Key]
    public int IdAnaliseRiscoOperacional { get; set; }

    /// <summary>
    /// Codigo da analise de risco operacional (ARO).
    /// </summary>
    [MaxLength(40)]
    public string? CodigoAro { get; set; }

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
    /// Anexos associados a analise.
    /// </summary>
    public virtual ICollection<AnnexModel> Anexos { get; set; } = new List<AnnexModel>();
}