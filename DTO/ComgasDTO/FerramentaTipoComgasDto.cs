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

    [SheetColumn("Descrição Ferramenta")]
    public string? Descricao { get; set; }

    [SheetColumn("Quantidade Disponível")]
    public int QuantidadeDisp { get; set; }

    [SheetColumn("Id Tipo Manutenção")]
    public int TipoManutencaoId { get; set; }

    [SheetColumn("Tipo ferramenta")]
    public string? TipoFerramenta { get; set; }

    [SheetColumn("Quantidade")]
    public int Quantidade { get; set; }

    [SheetColumn("Centro trabalho")]
    public int? CentroTrabalhoId { get; set; }

    [SheetColumn("Loc.instalacao")]
    public string? LocalInstalacaoCodigo { get; set; }

    [SheetColumn("Fabricante")]
    public int? FabricanteId { get; set; }

    [SheetColumn("Peso bruto")]
    public decimal? PesoBruto { get; set; }

    [SheetColumn("Peso liquido")]
    public decimal? PesoLiquido { get; set; }

    [SheetColumn("Volume")]
    public decimal? Volume { get; set; }

    [SheetColumn("Dimensao")]
    public decimal? Dimensao { get; set; }

    [SheetColumn("Valor")]
    public decimal? Valor { get; set; }

    [SheetColumn("Garantia ate")]
    public DateTime? GarantiaAte { get; set; }
}

#pragma warning restore CS1591
