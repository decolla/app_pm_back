using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class CtmModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int IdGPM { get; set; }

    [MaxLength(100)]
    public required string AreaCTM { get; set; }

    [MaxLength(300)]
    public string? Descricao { get; set; }

    public int IdCPM { get; set; }

    [ForeignKey(nameof(IdCPM))]
    public virtual CpmModel? CPM { get; set; }
}
