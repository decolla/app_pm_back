using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de centro de planejamento de manutencao Comgas.
/// </summary>
public class CentroPlanejamentoManutencaoComgasDto
{
    [SheetColumn("Id CPM")]
    public int IdCpm { get; set; }

    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    [SheetColumn("Organizacao")]
    public string? Organizacao { get; set; }

    [SheetColumn("Localizacao")]
    public string? Localizacao { get; set; }

    [SheetColumn("Localizacao Id")]
    public int? LocalizacaoId { get; set; }

    [SheetColumn("Descricao")]
    public string? Descricao { get; set; }
}

#pragma warning restore CS1591

