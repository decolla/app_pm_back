using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tipo de estrategia de manutencao.
/// </summary>
public class TipoEstrategiaComgasDto
{
    [SheetColumn("Id Tipo Estratégia")]
    public int IdTipoEstrategia { get; set; }

    [SheetColumn("Dias")]
    public int? Dias { get; set; }

    [SheetColumn("Tolerância positiva")]
    public double? ToleranciaPositiva { get; set; }

    [SheetColumn("Tolerância negativa")]
    public double? ToleranciaNegativa { get; set; }

    [SheetColumn("Continua")]
    public bool? Continua { get; set; }

    [SheetColumn("Categoria")]
    public string? Categoria { get; set; }

    [SheetColumn("Id Estratégia")]
    public int? IdEstrategia { get; set; }

    [SheetColumn("Id Tipo Manutenção")]
    public int? IdTipoManutencao { get; set; }

    [SheetColumn("Id Tipo CEP")]
    public int? IdTipoCep { get; set; }
}

#pragma warning restore CS1591
