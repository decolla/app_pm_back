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

    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    [SheetColumn("Id CPM")]
    public int? IdCpm { get; set; }

    [SheetColumn("Status")]
    public string? Status { get; set; }
}
