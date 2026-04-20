using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de localizacao administrativa da Comgas.
/// </summary>
public class LocalizacaoComgasDto
{
    [SheetColumn("Id Localização")]
    public int IdLocalizacao { get; set; }

    [SheetColumn("Cen.localiz.")]
    public string? CenLocalizacao { get; set; }

    [SheetColumn("Localização")]
    public string? CodigoLocalizacao { get; set; }

    [SheetColumn("Denominação")]
    public string? Nome { get; set; }

    [SheetColumn("Em Garantia")]
    public bool? EmGarantia { get; set; }

    [SheetColumn("Área operacion.")]
    public string? AreaOperacional { get; set; }

    [SheetColumn("Código ABC")]
    public string? CodigoAbc { get; set; }

    [SheetColumn("Campo ordenação")]
    public string? CampoOrdenacao { get; set; }

    [SheetColumn("Nº endereço")]
    public int? EnderecoId { get; set; }
}

#pragma warning restore CS1591
