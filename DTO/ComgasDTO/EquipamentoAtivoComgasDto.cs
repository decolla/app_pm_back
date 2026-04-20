using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

/// <summary>
/// DTO de equipamento ativo da Comgas.
/// </summary>
public class EquipamentoAtivoComgasDto
{
    [SheetColumn("Objeto")]
    public string? TagEquipamento { get; set; }

    [SheetColumn("TIPO")]
    public string? Tipo { get; set; }

    [SheetColumn("Denominação")]
    public string? Denominacao { get; set; }

    [SheetColumn("Status")]
    public string? Status { get; set; }

    [SheetColumn("Valido desde")]
    public DateTime? ValidoDesde { get; set; }

    [SheetColumn("Valido até")]
    public DateTime? ValidoAte { get; set; }

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
    public string? FabricanteNomeExcel { get; set; }
    
    public int? FabricanteId { get; set; }

    [SheetColumn("Localização")]
    public string? LocalizacaoNomeExcel { get; set; }
    
    public int? LocalizacaoId { get; set; }

    [SheetColumn("Nº endereço")]
    public int? EnderecoId { get; set; }

    [SheetColumn("Imobilizado")]
    public string? ImobilizadoTag { get; set; }

    [SheetColumn("Grp.plnj.PM")]
    public int? GpmId { get; set; }

    [SheetColumn("Centro trabalho")]
    public int? CentroTrabalhoId { get; set; }

    [SheetColumn("Loc.instalação")]
    public string? LocalInstalacaoCodigo { get; set; }
}

