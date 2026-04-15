using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

public class ItemManutencaoComgasModel
{
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
    [SheetColumn("Loc.instalação")]
    public string LocInstalacao { get; set; }
    
    /// <summary>
    /// Equipamento
    /// </summary>
    [SheetColumn("Equipamento")]
    public string? Equipamento { get; set; }
}