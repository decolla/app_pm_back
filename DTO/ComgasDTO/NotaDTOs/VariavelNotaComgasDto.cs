using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de variavel registrada na nota de manutencao.
/// </summary>
public class VariavelNotaComgasDto
{
    [SheetColumn("Id Variavel")]
    public int IdVariavel { get; set; }

    [SheetColumn("Nota")]
    public long NotaId { get; set; }

    [SheetColumn("Nome variavel")]
    public string? Nome { get; set; }

    [SheetColumn("Valor numerico")]
    public decimal? ValorNumerico { get; set; }

    [SheetColumn("Unidade medida")]
    public string? UnidadeMedida { get; set; }

    [SheetColumn("Comentarios")]
    public string? Comentarios { get; set; }
}

#pragma warning restore CS1591

