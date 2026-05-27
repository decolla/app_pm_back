using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de variavel registrada na nota de manutencao.
/// </summary>
public class VariavelNotaComgasDto
{
    // Identificacao
    [SheetColumn("Id Variavel")]
    public int IdVariavel { get; set; }

    [SheetColumn("Nota")]
    public long? IdNota { get; set; }

    [SheetColumn("Valor numerico")]
    public double? ValorNumerico { get; set; }

    [SheetColumn("Unidade medida")]
    public string? UnidadeMedida { get; set; }

    [SheetColumn("Comentarios")]
    public string? Comentarios { get; set; }
}

#pragma warning restore CS1591

