using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tipo de estrategia de manutencao.
/// </summary>
public class TipoEstrategiaDto
{
    [SheetColumn("Id Tipo Estratégia")] public long IdTipoEstrategia { get; set; }

    [SheetColumn("Dias")] public int? Dias { get; set; }

    [SheetColumn("Tolerância positiva")] public double? ToleranciaPositiva { get; set; }

    [SheetColumn("Tolerância negativa")] public double? ToleranciaNegativa { get; set; }

    [SheetColumn("Continua")] public bool? Continua { get; set; }

    [SheetColumn("Categoria")] public CategoriaTipoManutencao Categoria { get; set; }

    [SheetColumn("Id Estratégia")] public long? IdEstrategia { get; set; }

    [SheetColumn("Id Tipo Manutenção")] public long? IdTipoManutencao { get; set; }

    [SheetColumn("Id Tipo CEP")] public long? IdTipoCep { get; set; }
}

#pragma warning restore CS1591