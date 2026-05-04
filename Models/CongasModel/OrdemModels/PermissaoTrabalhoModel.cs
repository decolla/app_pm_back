using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models;
using app_pm_back.Models.Domain;

/// <summary>
/// Cadastro de permissao de trabalho vinculada a uma ordem.
/// </summary>
public class PermissaoTrabalhoModel
{
    /// <summary>
    /// Identificador interno da permissao.
    /// </summary>
    [Key]
    public int IdPermissaoTrabalho { get; set; }

    /// <summary>
    /// Codigo da permissao de trabalho (PT).
    /// </summary>
    [MaxLength(40)]
    public string? CodigoPt { get; set; }

    /// <summary>
    /// Data de criacao da permissao.
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Indicador de permissao concedida.
    /// </summary>
    public bool? PermissaoConcedida { get; set; }

    /// <summary>
    /// TAG exibida para referencia da ordem.
    /// </summary>
    [MaxLength(60)]
    public string? TagOrdem { get; set; }

    /// <summary>
    /// Ordem relacionada a permissao.
    /// </summary>
    public long? OrdemId { get; set; }

    /// <summary>
    /// Ordem relacionada.
    /// </summary>
    [ForeignKey(nameof(OrdemId))]
    public virtual OrdemManutencaoModel? Ordem { get; set; }

    /// <summary>
    /// PM_Anexos associados a permissao.
    /// </summary>
    public virtual ICollection<AnnexModel> Anexos { get; set; } = new List<AnnexModel>();
}