using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class OperacaoModel
{
    [Key]
    public int IdOperacao { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    [MaxLength(500)]
    public string? Descricao { get; set; }

    public double Duracao { get; set; }

    public int NdeProfissionais { get; set; }

    public DateTime? InicioEsperado { get; set; }

    public DateTime? FimEsperado { get; set; }

    public int? IdCTM { get; set; }

    public int? IdTipoManutencao { get; set; }

    public long? IdOrdem { get; set; }

    [ForeignKey(nameof(IdCTM))]
    public virtual CtmModel? Ctm { get; set; }

    [ForeignKey(nameof(IdTipoManutencao))]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }

    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemModel? Ordem { get; set; }

    public virtual ICollection<FerramentaModel> Ferramentas { get; set; } = new List<FerramentaModel>();
}
