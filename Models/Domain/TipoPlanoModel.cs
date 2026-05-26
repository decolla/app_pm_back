using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class TipoPlanoModel
{
    [Key]
    public int IdTipoPlano { get; set; }

    public bool Visivel { get; set; }

    public int Dias { get; set; }

    public double ToleranciaPositiva { get; set; }

    public double ToleranciaNegativa { get; set; }

    public bool Continua { get; set; }

    [MaxLength(100)]
    public required string Categoria { get; set; }

    public int IdTipoEstrategia { get; set; }

    public long IdPlano { get; set; }

    public int IdTipoManutencao { get; set; }

    public int IdTipoCep { get; set; }

    [ForeignKey(nameof(IdTipoEstrategia))]
    public virtual TipoEstrategiaModel? TipoEstrategia { get; set; }

    [ForeignKey(nameof(IdPlano))]
    public virtual PlanoModel? Plano { get; set; }

    [ForeignKey(nameof(IdTipoManutencao))]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }

    [ForeignKey(nameof(IdTipoCep))]
    public virtual TipoCepModel? TipoCep { get; set; }
}
