using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

/// <summary>
/// DTO de estrategia de manutencao .
/// </summary>
public class EstrategiaManutencaoDto
{
    [SheetColumn("Id Estratégia")] public long IdEstrategia { get; set; }

    [SheetColumn("Estratégia")] public string? Nome { get; set; }

    [SheetColumn("Ciclos")] public int? Ciclos { get; set; }

    [SheetColumn("Habilitada")] public bool Habilitada { get; set; }

    [SheetColumn("Número ciclos")] public int? NumeroCiclos { get; set; }

    [SheetColumn("Ciclo contínuo")] public bool? CicloContinuo { get; set; }

    [SheetColumn("Tipo ativo aplicável")] public TiposLI? TipoLi { get; set; }

    [SheetColumn("Id Escopo")] public string? IdEscopo { get; set; }
    
    public virtual List<TipoEstrategiaDto>? TiposEstrategia { get; set; } = new List<TipoEstrategiaDto>();
}