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

    [SheetColumn("Nome tipo")]
    public string? NomeTipo { get; set; }

    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    [SheetColumn("Dias")]
    public int Meses { get; set; }

    [SheetColumn("Tolerância positiva")]
    public int ToleranciaPositiva { get; set; }

    [SheetColumn("Unidade tolerância positiva")]
    public string? UnidadeToleranciaPositiva { get; set; }

    [SheetColumn("Tolerância negativa")]
    public int ToleranciaNegativa { get; set; }

    [SheetColumn("Unidade tolerância negativa")]
    public string? UnidadeToleranciaNegativa { get; set; }

    [SheetColumn("Id Estratégia")]
    public int? EstrategiaId { get; set; }
}

#pragma warning restore CS1591
