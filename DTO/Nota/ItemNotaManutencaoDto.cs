using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de item de nota de manutenção.
/// </summary>
public class ItemNotaManutencaoDto
{
    // Identificacao
    [SheetColumn("Id Item")] public long IdItemNota { get; set; }

    [SheetColumn("Nota")] public long IdNota { get; set; }

    // Descricao tecnica
    [SheetColumn("Code parte obj.")] public string? Componente { get; set; }

    [SheetColumn("Descrição")] public string? Sintoma { get; set; }

    [SheetColumn("Txt.code causa")] public string? Causa { get; set; }
}

#pragma warning restore CS1591