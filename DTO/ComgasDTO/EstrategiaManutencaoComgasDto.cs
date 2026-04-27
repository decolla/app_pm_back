using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.ComgasDTO;

/// <summary>
/// DTO de estrategia de manutencao Comgas.
/// </summary>
public class EstrategiaManutencaoComgasDto
{
    [SheetColumn("Id Estratégia")]
    public int IdEstrategia { get; set; }

    [SheetColumn("Estratégia")]
    public string? Nome { get; set; }

    [SheetColumn("Denominação")]
    public string? Denominacao { get; set; }

    [SheetColumn("Ciclos")]
    public int Ciclos { get; set; }

    [SheetColumn("UnidadeCiclos")]
    public UnidadeCiclo UnidadeCiclo { get; set; }

    [SheetColumn("Habilitada")]
    public bool Habilitada { get; set; }

    [SheetColumn("Número ciclos")]
    public int NumeroCiclos { get; set; }

    [SheetColumn("Tipo ativo aplicável")]
    public string? TipoAtivoAplicavel { get; set; }
}

