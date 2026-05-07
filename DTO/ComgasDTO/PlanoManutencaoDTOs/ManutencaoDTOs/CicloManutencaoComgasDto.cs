using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de ciclo de manutencao Comgas.
/// </summary>
public class CicloManutencaoComgasDto
{
    [SheetColumn("Id Ciclo")]
    public int IdCiclo { get; set; }

    [SheetColumn("Ciclo")]
    public int Valor { get; set; }

    [SheetColumn("Unidade")]
    public string? Unidade { get; set; }

    [SheetColumn("Nível manutenção")]
    public string? NivelManutencao { get; set; }

    [SheetColumn("Offset")]
    public int? Offset { get; set; }

    [SheetColumn("Plano manut.")]
    public long PlanoManutencaoCodigo { get; set; }
}

