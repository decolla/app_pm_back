using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de plano de manutencao .
/// </summary>
public class PlanoManutencaoDto
{
    [SheetColumn("Plano manut.")] public long IdPlano { get; set; }

    [SheetColumn("Nome plano")] public string? Nome { get; set; }

    [SheetColumn("Ativo")] public string? NomeAtivo { get; set; }

    [SheetColumn("Prioridade")] public Prioridade? Prioridade { get; set; }

    [SheetColumn("Gerar automaticamente")] public GerarAutomaticamente? GerarAutomaticamente { get; set; }

    [SheetColumn("Dt.inicio")] public DateTime? DataInicios { get; set; }

    [SheetColumn("Data conclusão")] public DateTime? DataConclusao { get; set; }

    [SheetColumn("Antecedência")] public int? Antecedencia { get; set; }

    [SheetColumn("Id Estratégia")] public int? IdEstrategia { get; set; }

    // Ativo externo (apenas o ID).
    [SheetColumn("Id Ativo")] public ComponentType? IdAtivo { get; set; }
}

#pragma warning restore CS1591