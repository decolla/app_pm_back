using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class DadosRenovacaoModel
{
    [Key]
    public int IdDados { get; set; }

    public DateTime UltimaRenovacao { get; set; }

    [MaxLength(100)]
    public required string TipoUR { get; set; }

    public double Confiabilidade { get; set; }

    public int IdAtivo { get; set; } // Stores Ativo ID only

    public long IdNota { get; set; }

    public long IdOrdem { get; set; }

    public int IdUsuario { get; set; }

    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }

    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemModel? Ordem { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual UsuarioModel? Usuario { get; set; }
}
