using app_pm_back.Attributes;

namespace app_pm_back.DTO;

/// <summary>
/// DTO de grupo de planejamento de manutencao.
/// </summary>
public class GrupoPlanejamentoManutencaoDto
{
    [SheetColumn("Id GPM")] public long IdGpm { get; set; }

    [SheetColumn("Descrição GPM")] public string? Descricao { get; set; }

    [SheetColumn("Nome")] public string? Nome { get; set; }

    [SheetColumn("Id CPM")] public long? IdCpm { get; set; }
}