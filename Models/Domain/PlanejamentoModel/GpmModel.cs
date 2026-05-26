using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class GpmModel
{
    [Key]
    public int IdGPM { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    [MaxLength(300)]
    public string? Descricao { get; set; }

    public int IdCPM { get; set; }

    [ForeignKey(nameof(IdCPM))]
    public virtual CpmModel? CPM { get; set; }

    public virtual ICollection<UsuarioModel> Usuarios { get; set; } = new List<UsuarioModel>();
}
