using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de centro de trabalho da Comgas.
/// </summary>
public class CentroTrabalhoComgasDto
{
    // Identificador usado como chave do CTM (corresponde ao IdGPM no modelo).
    [SheetColumn("Id Centro Trabalho")]
    public int IdGpm { get; set; }

    // Area do CTM (normalmente codigo do centro de trabalho).
    [SheetColumn("Centro trabalho")]
    public string? AreaCTM { get; set; }

    [SheetColumn("Denominação")]
    public string? Nome { get; set; }

    [SheetColumn("Id CPM")]
    public int? IdCpm { get; set; }

    [SheetColumn("DescricaoBreve")]
    public string? Descricao { get; set; }
}
