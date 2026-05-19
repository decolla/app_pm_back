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

    [SheetColumn("Denominação")]
    public string? Nome { get; set; }

    [SheetColumn("Cen.localiz.")]
    public string? CenManutencao { get; set; }

    [SheetColumn("Localização")]
    public string? CodigoLocalizacao { get; set; }

    [SheetColumn("CEP")]
    public string? CEP { get; set; }

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

    [SheetColumn("Organizacao")]
    public string? Organizacao { get; set; }

    [SheetColumn("Centro manutencao")]
    public string? CentroManutencao { get; set; }

    [SheetColumn("Estado")]
    public string? Estado { get; set; }

    [SheetColumn("Cidade")]
    public string? Cidade { get; set; }

    [SheetColumn("Bairro")]
    public string? Bairro { get; set; }

    [SheetColumn("Rua")]
    public string? Rua { get; set; }

    [SheetColumn("Numero")]
    public string? Numero { get; set; }

    [SheetColumn("Complemento")]
    public string? Complemento { get; set; }

    [SheetColumn("Telefone")]
    public string? Telefone { get; set; }

    [SheetColumn("Denominacao local")]
    public string? Denominacao { get; set; }

    [SheetColumn("Descricao denominacao")]
    public string? DescricaoDenominacao { get; set; }
}

#pragma warning restore CS1591
