using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de centro de trabalho da Comgas.
/// </summary>
public class CentroTrabalhoComgasDto
{
    [SheetColumn("Id Centro Trabalho")]
    public int IdCentroTrabalho { get; set; }

    [SheetColumn("Centro trabalho")]
    public string? Codigo { get; set; }

    [SheetColumn("Denominação")]
    public string? Nome { get; set; }

    [SheetColumn("Centro planej.")]
    public string? CentroPlanejamento { get; set; }
}

