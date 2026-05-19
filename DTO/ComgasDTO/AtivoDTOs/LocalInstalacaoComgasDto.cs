using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de local de instalacao da Comgas.
/// </summary>
public class LocalInstalacaoComgasDto
{
    [SheetColumn("Loc.instalação")]
    public int? CodigoLocalInstalacao { get; set; }

    [SheetColumn("Denominação")]
    public string? Denominacao { get; set; }

    [SheetColumn("Status sistema")]
    public string? Status { get; set; }

    [SheetColumn("Sistema identificado")]
    public string? SistemaIdentificado { get; set; }

    [SheetColumn("Código estrutura")]
    public string? CodigoEstrutura { get; set; }

    [SheetColumn("Categoria")]
    public string? Categoria { get; set; }

    [SheetColumn("Classe")]
    public string? Classe { get; set; }

    [SheetColumn("Tipo de objeto")]
    public string? TipoObjeto { get; set; }

    [SheetColumn("GrpAutorizações")]
    public string? GrupoAutorizacoes { get; set; }

    [SheetColumn("Peso")]
    public decimal? Peso { get; set; }

    [SheetColumn("Nº inventário")]
    public string? NumeroInventario { get; set; }

    [SheetColumn("Tipo de nota")]
    public string? TipoNotaPadrao { get; set; }

    [SheetColumn("Valor aquisição")]
    public decimal? ValorAquisicao { get; set; }

    [SheetColumn("Data aquisição")]
    public DateTime? DataAquisicao { get; set; }

    [SheetColumn("Fabricante")]
    public int? FabricanteId { get; set; }

    [SheetColumn("Localização")]
    public int? LocalizacaoId { get; set; }

    [SheetColumn("Nº endereço")]
    public int? EnderecoId { get; set; }

    [SheetColumn("Imobilizado")]
    public string? ImobilizadoTag { get; set; }

    [SheetColumn("Grp.plnj.PM")]
    public int? GpmId { get; set; }

    [SheetColumn("Centro trabalho")]
    public int? CentroTrabalhoId { get; set; }
}

#pragma warning restore CS1591
