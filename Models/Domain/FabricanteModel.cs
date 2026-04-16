using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Fabricante vinculado a ativos de manutencao.
/// </summary>
public class FabricanteModel
{
    /// <summary>
    /// Identificador interno do fabricante.
    /// </summary>
    [Key]
    public int IdFabricante { get; set; }

    /// <summary>
    /// Codigo externo do fabricante.
    /// </summary>
    [MaxLength(50)]
    public string? Codigo { get; set; }

    /// <summary>
    /// Nome comercial do fabricante.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Locais de instalacao que utilizam este fabricante.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();

    /// <summary>
    /// Equipamentos fabricados por este fornecedor.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();
}

