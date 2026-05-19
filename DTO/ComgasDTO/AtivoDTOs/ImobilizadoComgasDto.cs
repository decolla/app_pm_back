using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

/// <summary>
/// DTO de imobilizado da Comgas.
/// </summary>
public class ImobilizadoComgasDto
{
    [SheetColumn("Tag Imobilizado")]
    public string? TagImobilizado { get; set; }

    public string? AtivoTag { get; set; }

    [SheetColumn("Denominação")]
    public string? Denominacao { get; set; }

    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    [SheetColumn("Nº de série")]
    public string? NumeroSerie { get; set; }

    [SheetColumn("Nº inventário")]
    public string? NumeroInventario { get; set; }

    [SheetColumn("Quantidade")]
    public decimal? Quantidade { get; set; }

    [SheetColumn("Data aquisição")]
    public DateTime? DataAquisicao { get; set; }

    [SheetColumn("Data incorporação")]
    public DateTime? DataIncorporacao { get; set; }

    [SheetColumn("Valor compra")]
    public decimal? ValorCompra { get; set; }

    [SheetColumn("Depreciação por ano")]
    public decimal? DepreciacaoAno { get; set; }

    [SheetColumn("Valor atual")]
    public decimal? ValorAtual { get; set; }

    [SheetColumn("Nome imobilizado")]
    public string? NomeImobilizado { get; set; }

    [SheetColumn("Informacao adicional")]
    public string? InformacaoAdicional { get; set; }
}
