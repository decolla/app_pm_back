using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de grupo de planejamento de manutencao.
/// </summary>
public class GrupoPlanejamentoManutencaoComgasDto
{
    [SheetColumn("Id GPM")]
    public int IdGpm { get; set; }

    [SheetColumn("Grp.plnj.PM")]
    public string? Codigo { get; set; }

    [SheetColumn("Descrição GPM")]
    public string? Descricao { get; set; }
}

