using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de fabricante da Comgas.
/// </summary>
public class FabricanteComgasDto
{
    [SheetColumn("Id Fabricante")]
    public int IdFabricante { get; set; }

    [SheetColumn("Código fabricante")]
    public string? Codigo { get; set; }

    [SheetColumn("FABRICANTE")]
    public string? Nome { get; set; }
}

