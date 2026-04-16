using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

/// <summary>
/// Item de manutencao associado a um plano no contexto COMGAS.
/// </summary>
public class ItemManutencaoComgasModel
{
    /// <summary>
    /// Identificador interno do item.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdItem {get; set;}
    
    /// <summary>
    /// Item de manutenção
    /// </summary>
    [SheetColumn("Item manut.")]
    public long? ItemManut { get; set; }
    
    /// <summary>
    /// Local de instalação do ativo
    /// </summary>
    /// <example>CG_05_CON_IND_CRM00073</example>
    [Required]
    [MaxLength(60)]
    [SheetColumn("Loc.instalação")]
    public required string LocInstalacao { get; set; }
    
    /// <summary>
    /// Equipamento
    /// </summary>
    [MaxLength(60)]
    [SheetColumn("Equipamento")]
    public string? Equipamento { get; set; }
    
    /// <summary>
    /// Chave estrangeira para plano de manutenção
    /// </summary>
    public int? PlanoId { get; set; }

    /// <summary>
    /// Plano de manutencao relacionado ao item.
    /// </summary>
    [ForeignKey("PlanoId")]
    public virtual MaintancePlanComgasModel? PlanoRelacionado { get; set; }
}