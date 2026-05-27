using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

/// <summary>
/// DTO de imobilizado da Comgas.
/// </summary>
public class ImobilizadoComgasDto
{
    // Identificacao
    [SheetColumn("Tag Imobilizado")]
    public int? IdImobilizado { get; set; }

    [SheetColumn("Nome imobilizado")]
    public string? Nome { get; set; }

    [SheetColumn("Denominação")]
    public string? Denominacao { get; set; }

    [SheetColumn("Nº de série")]
    public string? Nserie { get; set; }

    // Dados patrimoniais
    [SheetColumn("Quantidade")]
    public int? Quantidade { get; set; }

    [SheetColumn("Data aquisição")]
    public DateTime? DataAquisicao { get; set; }

    [SheetColumn("Data incorporação")]
    public DateTime? DataIncorporacao { get; set; }

    [SheetColumn("Valor compra")]
    public decimal? ValorCompra { get; set; }

    // Pode vir de outra fonte; se ausente, usa-se o default no mapeamento.
    [SheetColumn("Moeda")]
    public string? Moeda { get; set; }

    [SheetColumn("Depreciação por ano")]
    public decimal? Depreciacao { get; set; }

    [SheetColumn("Valor atual")]
    public decimal? ValorAtual { get; set; }

    [SheetColumn("Informacao adicional")]
    public string? InfoAdicionais { get; set; }
}
