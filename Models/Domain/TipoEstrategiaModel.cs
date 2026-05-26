using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class TipoEstrategiaModel
{
    [Key]
    public int IdTipoEstrategia { get; set; }

    public int Dias { get; set; }

    public double ToleranciaPositiva { get; set; }

    public double ToleranciaNegativa { get; set; }

    public bool Continua { get; set; }

    [MaxLength(100)]
    public required string Categoria { get; set; }

    public int IdEstrategia { get; set; }

    public int IdTipoManutencao { get; set; }

    public int IdTipoCep { get; set; }

    [ForeignKey(nameof(IdEstrategia))]
    public virtual EstrategiaModel? Estrategia { get; set; }

    [ForeignKey(nameof(IdTipoManutencao))]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }

    [ForeignKey(nameof(IdTipoCep))]
    public virtual TipoCepModel? TipoCep { get; set; }
}
