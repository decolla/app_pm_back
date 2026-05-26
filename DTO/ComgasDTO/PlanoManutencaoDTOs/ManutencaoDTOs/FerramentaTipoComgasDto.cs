using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ferramenta vinculada ao tipo de manutencao.
/// </summary>
public class FerramentaTipoComgasDto
{
    [SheetColumn("Id Ferramenta")]
    public int IdFerramenta { get; set; }

    [SheetColumn("Selecionada")]
    public bool Checkbox { get; set; }

    [SheetColumn("Nome Ferramenta")]
    public string? Nome { get; set; }

    public string? Categoria { get; set; }

    public string? TextoBreve { get; set; }

    [SheetColumn("Descrição Ferramenta")]
    public string? Descricao { get; set; }

    [SheetColumn("Quantidade Disponível")]
    public int QuantidadeDisp { get; set; }

    [SheetColumn("Tipo ferramenta")]
    public string? TipoFerramenta { get; set; }

    [SheetColumn("Quantidade")]
    public int Quantidade { get; set; }

    [SheetColumn("Loc.instalacao")]
    public int? IdLocalInstalacao { get; set; }

    [SheetColumn("Fabricante")]
    public int? FabricanteId { get; set; }

    [SheetColumn("Peso bruto")]
    public decimal? PesoBruto { get; set; }

    public string? PesoBrutoUnidade { get; set; }

    [SheetColumn("Peso liquido")]
    public decimal? PesoLiquido { get; set; }

    public string? PesoLiquidoUnidade { get; set; }

    [SheetColumn("Volume")]
    public decimal? Volume { get; set; }

    public string? VolumeUnidade { get; set; }

    [SheetColumn("Dimensao")]
    public decimal? Dimensao { get; set; }

    public string? DimensaoUnidade { get; set; }

    [SheetColumn("Valor")]
    public decimal? ValorCompra { get; set; }

    public DateTime? DataAquisicao { get; set; }
    
}

#pragma warning restore CS1591
