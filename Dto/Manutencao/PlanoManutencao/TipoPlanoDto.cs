using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tipo de plano .
/// </summary>
public class TipoPlanoDto
{
    [SheetColumn("Id Tipo Plano")] public long IdTipoPlano { get; set; }

    [SheetColumn("Visivel")] public bool? Visivel { get; set; }

    [SheetColumn("Dias")] public int? Dias { get; set; }

    [SheetColumn("Tolerância positiva")] public double? ToleranciaPositiva { get; set; }

    [SheetColumn("Tolerância negativa")] public double? ToleranciaNegativa { get; set; }

    [SheetColumn("Continua")] public bool? Continua { get; set; }

    [SheetColumn("Categoria")] public string? Categoria { get; set; }

    [SheetColumn("Id Tipo Estratégia")] public long? IdTipoEstrategia { get; set; }

    [SheetColumn("Plano manut.")] public long? IdPlano { get; set; }

    [SheetColumn("Id Tipo Manutenção")] public long? IdTipoManutencao { get; set; }

    [SheetColumn("Id Tipo CEP")] public long? IdTipoCep { get; set; }
}

#pragma warning restore CS1591