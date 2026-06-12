using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de centro de planejamento de manutencao .
/// </summary>
public class CentroPlanejamentoManutencaoDto
{
    [SheetColumn("Id CPM")] public int IdCPM { get; set; }

    [SheetColumn("Nome")] public string? NomeCentro { get; set; }

    [SheetColumn("Organizacao")] public string? Organizacao { get; set; }

    [SheetColumn("DescricaoBreve")] public string? Descricao { get; set; }

    // Escopo externo (apenas o ID)
    [SheetColumn("Id Escopo")] public int? IdEscopo { get; set; }
}

#pragma warning restore CS1591