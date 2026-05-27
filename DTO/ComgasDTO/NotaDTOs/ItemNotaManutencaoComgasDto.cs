using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de item de nota de manutencao Comgas.
/// </summary>
public class ItemNotaManutencaoComgasDto
{
    // Identificacao
    [SheetColumn("Id Item")]
    public int IdItemNota { get; set; }

    [SheetColumn("Nota")]
    public long IdNota { get; set; }

    // Descricao tecnica
    [SheetColumn("Code parte obj.")]
    public string? Componente { get; set; }

    [SheetColumn("Sintoma")]
    public string? Sintoma { get; set; }

    [SheetColumn("Causa")]
    public string? Causa { get; set; }
}

#pragma warning restore CS1591
