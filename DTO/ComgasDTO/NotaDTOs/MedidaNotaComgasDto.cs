using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de medidas sugeridas para notas.
/// </summary>
public class MedidaNotaComgasDto
{
    [SheetColumn("Id Medida")]
    public int IdMedida { get; set; }

    [SheetColumn("Nota")]
    public long NotaId { get; set; }

    [SheetColumn("Texto code ação")]
    public string? Descricao { get; set; }
}

