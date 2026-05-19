using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Cadastro da matriz LIBRA vinculada a equipamentos.
/// </summary>
public class MatrizLibraModel
{
    /// <summary>
    /// Identificador interno da matriz.
    /// </summary>
    [Key]
    public int IdMatrizLibra { get; set; }

    /// <summary>
    /// Codigo da matriz LIBRA.
    /// </summary>
    [MaxLength(40)]
    public string? CodigoLibra { get; set; }

    /// <summary>
    /// Data de criacao da matriz.
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// TAG do equipamento associado.
    /// </summary>
    [MaxLength(60)]
    public string? EquipamentoTag { get; set; }

    /// <summary>
    /// Data de validade informada.
    /// </summary>
    public DateTime? ValidadeAte { get; set; }

    /// <summary>
    /// Equipamento associado.
    /// </summary>
    [ForeignKey(nameof(EquipamentoTag))]
    public virtual EquipamentoAtivoModel? Equipamento { get; set; }

    /// <summary>
    /// PM_Anexos associados a matriz.
    /// </summary>
    public virtual ICollection<AnnexModel> Anexos { get; set; } = new List<AnnexModel>();
}
