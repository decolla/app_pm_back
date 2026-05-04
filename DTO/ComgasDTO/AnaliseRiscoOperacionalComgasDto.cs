using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de analise de risco operacional (ARO) Comgas.
/// </summary>
public class AnaliseRiscoOperacionalComgasDto
{
    [SheetColumn("Codigo ARO")]
    public string? CodigoAro { get; set; }

    [SheetColumn("Data criacao")]
    public DateTime? DataCriacao { get; set; }

    [SheetColumn("Resultado analise")]
    public string? ResultadoAnalise { get; set; }

    [SheetColumn("TAG ordem")]
    public string? TagOrdem { get; set; }

    [SheetColumn("Ordem")]
    public long? OrdemId { get; set; }
}

#pragma warning restore CS1591

