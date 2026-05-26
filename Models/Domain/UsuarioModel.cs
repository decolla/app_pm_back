using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

public class UsuarioModel
{
    [Key]
    public int IdUsuario { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    [MaxLength(20)]
    public required string CPF { get; set; }

    [MaxLength(500)]
    public string? Permissoes { get; set; }

    public int IdEscopo { get; set; } // Stores Escopo ID only

    public virtual ICollection<NotaModel> Notas { get; set; } = new List<NotaModel>();
    public virtual ICollection<DadosRenovacaoModel> DadosRenovacao { get; set; } = new List<DadosRenovacaoModel>();
    public virtual ICollection<AlmoxarifadoModel> Almoxarifados { get; set; } = new List<AlmoxarifadoModel>();
    public virtual ICollection<GpmModel> Gpms { get; set; } = new List<GpmModel>();
    public virtual ICollection<CtmModel> Ctms { get; set; } = new List<CtmModel>();
}
