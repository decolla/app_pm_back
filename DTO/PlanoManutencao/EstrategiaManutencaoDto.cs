using app_pm_back.Attributes;

namespace app_pm_back.DTO;

/// <summary>
/// DTO de estrategia de manutencao .
/// </summary>
public class EstrategiaManutencaoDto
{
    [SheetColumn("Id Estratégia")] public int IdEstrategia { get; set; }

    [SheetColumn("Estratégia")] public string? Nome { get; set; }

    [SheetColumn("Ciclos")] public int? Ciclos { get; set; }

    [SheetColumn("Habilitada")] public bool Habilitada { get; set; }

    [SheetColumn("Número ciclos")] public int? NumeroCiclos { get; set; }

    [SheetColumn("Ciclo contínuo")] public bool? CicloContinuo { get; set; }

    [SheetColumn("Tipo ativo aplicável")] public string? TipoAtivoAplicavel { get; set; }

    // Escopo externo (apenas o ID).
    [SheetColumn("Id Escopo")] public int? IdEscopo { get; set; }
}