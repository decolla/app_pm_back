using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de item do plano de manutencao Comgas.
/// </summary>
public class ItemPlanoManutencaoComgasDto
{
    [SheetColumn("Id Item Plano")]
    public int IdItemPlano { get; set; }

    [SheetColumn("Item manut.")]
    public string? CodigoItem { get; set; }

    [SheetColumn("Plano manut.")]
    public long? PlanoManutencaoCodigo { get; set; }

    [SheetColumn("Equipamento")]
    public string? EquipamentoTag { get; set; }

    [SheetColumn("Loc.instalação")]
    public string? LocalInstalacaoCodigo { get; set; }
}

#pragma warning restore CS1591
