using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

public class EstrategiaComgasModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int idEstrategia { get; set; }
    
    /// <summary>
    /// Estratégia
    /// </summary>
    [SheetColumn("Estratégia")]
    public string? Estrategia { get; set; }
}